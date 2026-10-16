using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace RCDWeb.Domain.Tests
{
    /// <summary>
    /// Test di regressione di sicurezza: falliscono se nel repository vengono reintrodotte
    /// credenziali, host di infrastruttura interna, dati personali o binari non ridistribuibili.
    ///
    /// Corrispondono ai criteri C1, C2 e C7 della rubrica in PLAN.md.
    /// </summary>
    public class SecurityRegressionTests
    {
        /// <summary>Estensioni di file la cui presenza è vietata nel versionamento.</summary>
        private static readonly string[] ForbiddenBinaryNames =
        {
            "AreaCommonWeb.dll",
            "WebServiceModel.dll",
            "Kendo.Mvc.dll",
            "Telerik.Web.Mvc.dll",
            "itextsharp.dll",
            "Microsoft.Office.Interop.Word.dll",
            "Microsoft.Web.Services3.dll",
            "lib.zip",
        };

        /// <summary>
        /// Estensioni analizzate per la ricerca di segreti e riferimenti infrastrutturali.
        /// Sono esclusi i formati discorsivi (.md) e i metadati (.json): la verifica riguarda
        /// codice, configurazione, build e CI, cioè gli artefatti che vengono eseguiti.
        /// </summary>
        private static readonly string[] ScannedExtensions =
        {
            ".cs", ".config", ".cshtml", ".csproj", ".sln", ".js", ".bat", ".sh",
            ".yml", ".yaml", ".xml", ".txt", ".props", ".targets", ".ps1", ".cmd",
        };

        /// <summary>
        /// File esclusi dalla scansione perché contengono i pattern di ricerca stessi
        /// (questo file e il workflow CI), non valori reali.
        /// </summary>
        private static readonly string[] ExcludedFiles =
        {
            "tests/RCDWeb.Domain.Tests/SecurityRegressionTests.cs",
            ".github/workflows/ci.yml",
        };

        /// <summary>Directory escluse dalla scansione.</summary>
        private static readonly string[] ExcludedDirectories =
        {
            ".git", "bin", "obj", "packages", "node_modules", ".vs", "TestResults",
            // Alberi di asset di terze parti (jQuery, Kendo, Telerik, CSS vendor):
            // non sono codice del progetto e contengono falsi positivi.
            "Source/Scripts", "Source/Content",
        };

        /// <summary>
        /// Pattern di credenziali. Il placeholder esplicitamente previsto dalla configurazione
        /// è ammesso; qualunque altro valore assegnato a una password non lo è.
        /// </summary>
        private static readonly (string Name, string Pattern)[] SecretPatterns =
        {
            ("password assegnata", @"(?i)password\s*=\s*[^""'\s;<]{3,}"),
            ("pwd assegnata", @"(?i)\bpwd\s*=\s*[^""'\s;<]{3,}"),
            ("api key", @"(?i)(api[_-]?key|apikey|secret[_-]?key)\s*[:=]\s*[^""'\s;<]{8,}"),
            ("bearer token", @"(?i)bearer\s+[A-Za-z0-9\-._~+/]{20,}"),
            ("chiave privata", @"-----BEGIN [A-Z ]*PRIVATE KEY-----"),
            // Solo in contesto di connection string (';' oppure inizio riga),
            // così non intercetta le variabili 'uid' del codice JavaScript.
            ("connection string con utente", @"(?i)(?:^|;)\s*(?:user\s*id|uid)\s*=\s*(?!rcd_user|\$|<|%|\{)[A-Za-z0-9_]+(?=\s*;|\s*$|"")"),
        };

        /// <summary>Pattern di infrastruttura interna e dati personali.</summary>
        private static readonly (string Name, string Pattern)[] InfrastructurePatterns =
        {
            ("host interno", @"\bsrv[a-z]{3,}[0-9]{1,}\b"),
            ("dominio aziendale", @"example\.it|example\.it"),
            ("IP privato", @"\b(10\.\d{1,3}|172\.(1[6-9]|2[0-9]|3[01])|192\.168)\.\d{1,3}\.\d{1,3}\b"),
            ("share UNC interna", @"\\\\[A-Za-z0-9._-]{3,}\\[A-Za-z0-9._$-]{2,}"),
            ("endpoint TFS", @"tfs/|SccAuxPath|SccProjectName"),
            // Richiede una parte locale che inizi con un carattere alfanumerico:
            // esclude le espressioni Razor (@Html...) e i falsi positivi dei template.
            ("email reale", @"(?<![A-Za-z0-9@._%+-])[A-Za-z0-9][A-Za-z0-9._%+-]*@(?!example\.(?:com|org|net)\b|localhost\b)(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,24}\b"),
        };

        /// <summary>Placeholder legittimi che non costituiscono segreto.</summary>
        private static readonly string[] AllowedPlaceholders =
        {
            "CHANGE_ME_DB_PASSWORD",
            "removed",
            "removed",
            "<DB_PASSWORD>",
            "<SVC_USER>",
            "removed",
            "removed",
        };

        private static string RepositoryRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "PLAN.md")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "Source")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            throw new InvalidOperationException(
                "Radice del repository non trovata a partire da " + AppContext.BaseDirectory);
        }

        private static IEnumerable<string> ScannedFiles()
        {
            var root = RepositoryRoot();

            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                var relative = path.Substring(root.Length + 1).Replace('\\', '/');

                if (ExcludedDirectories.Any(d =>
                        relative.Equals(d, StringComparison.OrdinalIgnoreCase) ||
                        relative.StartsWith(d + "/", StringComparison.OrdinalIgnoreCase) ||
                        relative.Contains("/" + d + "/")))
                {
                    continue;
                }

                if (!ScannedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (ExcludedFiles.Contains(relative, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return path;
            }
        }

        private static string Sanitize(string content)
        {
            var result = content;

            foreach (var placeholder in AllowedPlaceholders)
            {
                result = result.Replace(placeholder, "<<PLACEHOLDER>>");
            }

            return result;
        }

        [Fact]
        public void C1_NoCredentialsInTrackedFiles()
        {
            var violations = new List<string>();

            foreach (var path in ScannedFiles())
            {
                var content = Sanitize(File.ReadAllText(path));

                foreach (var (name, pattern) in SecretPatterns)
                {
                    foreach (Match match in Regex.Matches(content, pattern))
                    {
                        // Una password vuota o un segnaposto sono ammessi.
                        if (Regex.IsMatch(match.Value, @"=\s*(""""|'')?\s*$")) continue;
                        if (match.Value.Contains("<<PLACEHOLDER>>")) continue;
                        if (Regex.IsMatch(match.Value, @"=\s*$")) continue;

                        violations.Add($"{name}: {Path.GetFileName(path)} -> {match.Value}");
                    }
                }
            }

            Assert.True(violations.Count == 0,
                "C1 fallito — credenziali rilevate:\n" + String.Join("\n", violations));
        }

        [Fact]
        public void C2_NoInternalInfrastructureReferences()
        {
            var violations = new List<string>();

            foreach (var path in ScannedFiles())
            {
                var fileName = Path.GetFileName(path);

                foreach (var (name, pattern) in InfrastructurePatterns)
                {
                    foreach (Match match in Regex.Matches(File.ReadAllText(path), pattern))
                    {
                        violations.Add($"{name}: {fileName} -> {match.Value}");
                    }
                }
            }

            Assert.True(violations.Count == 0,
                "C2 fallito — riferimenti a infrastruttura interna:\n" + String.Join("\n", violations));
        }

        [Fact]
        public void C7_NoNonRedistributableBinariesTracked()
        {
            // Non basta che il file non esista su disco: la verifica riguarda l'**indice git**.
            // I binari proprietari possono restare localmente (sono in .gitignore) ma non
            // devono mai essere versionati.
            var root = RepositoryRoot();
            var tracked = GitLsFiles(root);

            var violations = tracked
                .Where(p => ForbiddenBinaryNames.Contains(
                    Path.GetFileName(p), StringComparer.OrdinalIgnoreCase))
                .ToList();

            Assert.True(violations.Count == 0,
                "C7 fallito — binari non ridistribuibili versionati (" + violations.Count + "):\n" +
                String.Join("\n", violations));
        }

        /// <summary>Elenco dei file versionati, relativo alla radice del repository.</summary>
        private static List<string> GitLsFiles(string root)
        {
            var startInfo = new ProcessStartInfo("git", "ls-files")
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            using (var process = Process.Start(startInfo))
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Assert.True(process.ExitCode == 0,
                    "Impossibile eseguire 'git ls-files': git non disponibile o directory non versionata.");

                return output
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
        }

        [Fact]
        public void C10_NoDeadCodeDirectories()
        {
            var root = RepositoryRoot();
            var violations = new List<string>();

            foreach (var name in new[] { "old", "BuildProcessTemplates" })
            {
                if (Directory.Exists(Path.Combine(root, name))) violations.Add(name + "/");
            }

            var sourceDir = Path.Combine(root, "Source");
            var jobDir = Path.Combine(root, "RCDWeb.Job");

            foreach (var dir in new[] { sourceDir, jobDir }.Where(Directory.Exists))
            {
                // Viste duplicate: *.cshtml con suffisso _old o " - Old".
                violations.AddRange(
                    Directory.EnumerateFiles(dir, "*_old.cshtml", SearchOption.AllDirectories)
                             .Select(ToRelative));
                violations.AddRange(
                    Directory.EnumerateFiles(dir, "* - Old.cshtml", SearchOption.AllDirectories)
                             .Select(ToRelative));

                // Sorgenti C# con prefisso '_' (file di backup non compilati),
                // escludendo i *.generated.cs di RazorGenerator, che sono legittimi.
                violations.AddRange(
                    Directory.EnumerateFiles(dir, "_*.cs", SearchOption.AllDirectories)
                             .Where(p => !p.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase))
                             .Select(ToRelative));
            }

            // Cartelle di backup non compilate.
            foreach (var bak in new[] { Path.Combine(jobDir, "bak") }.Where(Directory.Exists))
            {
                violations.AddRange(Directory.EnumerateFiles(bak, "*", SearchOption.AllDirectories)
                                             .Select(ToRelative));
            }

            Assert.True(violations.Count == 0,
                "C10 fallito — codice morto presente (" + violations.Count + " file):\n" +
                String.Join("\n", violations.Take(50)));

            string ToRelative(string p) { return p.Substring(root.Length + 1).Replace('\\', '/'); }
        }

        [Fact]
        public void Repository_HasRequiredDocumentation()
        {
            var root = RepositoryRoot();

            foreach (var file in new[] { "LICENSE", "README.md", "SECURITY.md", "CONTRIBUTING.md", "CHANGELOG.md", "THIRD-PARTY.md" })
            {
                Assert.True(File.Exists(Path.Combine(root, file)), "File mancante: " + file);
            }
        }

        [Fact]
        public void Readme_HasAllRequiredSections()
        {
            var root = RepositoryRoot();
            var readme = File.ReadAllText(Path.Combine(root, "README.md"), Encoding.UTF8);

            var sections = new[]
            {
                "1. Panoramica e funzionalità",
                "2. Stack tecnologico",
                "3. Requisiti",
                "4. Installazione passo-passo",
                "5. Configurazione",
                "6. Uso e flusso applicativo",
                "7. Test",
                "8. Troubleshooting",
                "9. Licenza, crediti e sicurezza",
            };

            var missing = sections.Where(s => !readme.Contains(s)).ToList();

            Assert.True(missing.Count == 0,
                "Sezioni mancanti nel README: " + String.Join(", ", missing));
        }

        [Fact]
        public void License_IsMit()
        {
            var root = RepositoryRoot();
            var license = File.ReadAllText(Path.Combine(root, "LICENSE"), Encoding.UTF8);

            Assert.Contains("MIT License", license);
            Assert.Contains("WITHOUT WARRANTY OF ANY KIND", license);
        }
    }
}
