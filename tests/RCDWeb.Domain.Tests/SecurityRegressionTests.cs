using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
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

        /// <summary>Estensioni dei documenti Office Open XML analizzati dalla guardia C1c.</summary>
        private static readonly string[] OfficeExtensions = { ".docx", ".xlsx", ".pptx" };

        /// <summary>Estensioni immagine analizzate dalla guardia C1c.</summary>
        private static readonly string[] ImageExtensions =
        {
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".tif", ".tiff", ".ico", ".svg",
        };

        /// <summary>Estensioni binarie saltate dalla scansione completa dei token.</summary>
        private static readonly string[] BinaryExtensions =
        {
            ".dll", ".exe", ".pdb", ".zip", ".gz", ".tar", ".7z", ".rar",
            ".png", ".jpg", ".jpeg", ".gif", ".ico", ".bmp", ".webp", ".svg",
            ".docx", ".xlsx", ".xls", ".pdf", ".eot", ".ttf", ".woff", ".woff2", ".swf", ".map",
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
            // Il dominio aziendale e la ragione sociale NON sono qui in chiaro:
            // sono verificati per hash in C1b (token) e C1c (marchi e documenti).
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

        /// <summary>
        /// Guardia hash-based contro la reintroduzione dei valori compromessi storici.
        ///
        /// I valori originali NON sono scritti nel repository: qui compaiono solo i loro
        /// SHA-256. Il test tokenizza ogni file di testo e confronta l'hash di ciascun token
        /// con l'insieme degli hash noti. Vedi docs/evidence/07-git-history.md.
        /// </summary>
        [Fact]
        public void C1b_NoKnownCompromisedTokensAnywhere()
        {
            var digests = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
            "fb267217d5acc8cd33ea20712712ceec24ce8cd70e2949fcad1d60e31ed02c24",
            "62fcbcfa3468e4b095e9bc4736898573ecd41e4d5a993adc1df57bad33163b77",
            "a6ecdabbf4029d22045764c0aa7a251f17dcfeb799f3aa4117b201ba69110614",
            "0e3e815ab5f1b3bece555b81dde7c3f93b212a1feea22121fcc408cd37558dae",
            "999ce726e6a134ca5f411a59ab274a3c82ef3e304517771f6b2b2c461bb9c37e",
            "241dfeb846821b6ed7737bcdec09109d7e841e6bfe138e7bfd8ad4807f7b38db",
            "5d731049a4ace10cb8571974f68247d4bada89052c744fa788281ae9de546599",
            "f76cfb95a28e204ada0b53e1151d5c4fb055f062be60453b9cdce5774943fb65",
            "057e53e04b201d3f6b0734e9f663889a5c181000acf01d145e3f8ee9fd1d479d",
            "9f2bce9b49fb94eeeee50318e259ffa595337a1a925d6c9d865ed4538b10f434",
            "63622189e387b511142c4b580267471e098d9a0a9b6257ebda7b2ed29b68951a",
            "cf323cabc7c3c309b53b359301bb0d4d97108e4cdd91059fc948354a7dd40214",
            "41eecb6961d2667cf7f35e98f435389595dbe756fa86db504c41d39e5acf8ddc",
            "1bc3a46aaaccc511ea67724d764706fc20ea7a4d42df47339544c2b64cfcb54c",
            "0b7834549e32828ae585431a4adcd92fcec910cc75b8583b0cc9e620338ef685",
            "27e30ea28b5f1332def1e0ff6adc180e8522cbf4c0dfe08c64d5d2f7fdc3e045",
            "13c940ff05a028e56ed0fc39f443529275c64e231fa6814532a53316d2c9681d",
            "6e13135eb601bef08da14394d0b1baa4303cf784b94e4665810ef0623c3d1a98",
            "33e865d58ffa673525a743a0e8ad53dc26896aa7cc658f12967837123fa317fa",
            "eaf67e6f1689c64e268549a6eed37b977db742a811f6b3677d85391841973d17",
            "8cf90f26e9f69b80838ce5744f36c5e60440b2f1442a1c60d326f1f8b63a6fa9",
            "df6ffa97d13bad4ba5aee9f5e4c926cb7308eb481fbaa98aaa868cb9a1f6218b",
            "adf948d7b23a486c0b9f1b4508e5c2cf2e301239aaf2d10a350c1c1f0d78999d",
            "774c072f2521238f1f4521b797b5b1e7f77c336b1b0d62637fefec2510dfa448",
            "a20b90a7a410a00d8ab3118ac65ef21b65843c45d6cf5ff581abf83a2b9a36a3",
            "85eb24dec3b7e40c8bc87d06ca6756f243c95a63ab262efb45f5cc8df46a33ef",
            "2a467889a001a0c2dfa035f1422909461a971fc04b375adc1aa96c2bf460c056",
            "2251e53079977f09d7d6a1c9291ed440c3c911de6973ff02184f3d34afb7b585",
            "6a4515610158bf27b317d02c1ad978bf6f9d22cf6fb296bd643833bf78678a71",
            "d43f3a21caad6d9a30ac79bf82d0a9249eba8ff6cad343ab1a0f3f982bd3b7de",
            "43586832660f11b4846de4062f4e48b6c5dbaa7341c01008598f75ffbf953c1b",
            "560fafc318c05134e034a07da59312b47cfe6b49873a5dc1e09eb9dcf264f0a8",
            "ff6e77b5072ff4ffc71f66fabf3156d669fe520ffc6a97e15297b6ce692b3c27",
            "93ff7dc87635e6973a1db397a549bb9c336baf05b94c683dfbf135f2ffd7daec",
            "cf7e32b5f107d172e43b13e878e86e29a9e96ff3d3da533e5746c5d0f9db890c",
            "e8e0459a7ca1a851c1555d9d627561814aa908b38a8999c2fd40dcf0b1cff8f3",
            };

            var root = RepositoryRoot();
            var violations = new List<string>();

            foreach (var path in AllTextFiles(root))
            {
                if (Path.GetFileName(path).Equals("SecurityRegressionTests.cs", StringComparison.OrdinalIgnoreCase))
                {
                    // Contiene solo gli hash, mai i valori.
                    continue;
                }

                string content;
                try
                {
                    var info = new FileInfo(path);
                    if (info.Length > 2 * 1024 * 1024) continue;
                    content = File.ReadAllText(path);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (Match match in Regex.Matches(content, @"[A-Za-z0-9_@.$%+-]{3,}"))
                {
                    var hit = false;

                    foreach (var candidate in Candidates(match.Value))
                    {
                        if (!digests.Contains(Sha256(candidate))) continue;
                        hit = true;
                        break;
                    }

                    if (hit)
                    {
                        var relative = path.Substring(root.Length + 1).Replace('\\', '/');
                        violations.Add(relative + " -> token compromesso rilevato (valore non stampato)");
                    }
                }
            }

            Assert.True(violations.Count == 0,
                "C1b fallito - token compromessi reintrodotti:\n" + String.Join("\n", violations.Distinct()));
        }

        /// <summary>
        /// Guardia contro marchi, ragioni sociali, indirizzi, dati personali e loghi
        /// dell'istituto di credito di origine, dentro documenti Office e immagini.
        ///
        /// I valori vietati NON sono scritti nel repository: il test usa un insieme di
        /// SHA-256 e, per i documenti, un hash a finestra scorrevole che rileva il marcatore
        /// anche quando è immerso in una frase più lunga. Vedi docs/evidence/10-branding.md.
        /// </summary>
        [Fact]
        public void C1c_NoBankIdentityInOfficeDocumentsOrImages()
        {
            // Marcatore normalizzato (spazi rimossi, minuscolo) -> SHA-256
            var textDigests = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "2a467889a001a0c2dfa035f1422909461a971fc04b375adc1aa96c2bf460c056",
                "8921d2c8491b6220827774cddc28ca86dc4cd878b2810b68db6d3818e8116989",
                "c3ed0c3691e45d1d6cabdc06f7a66aed564688c53455ee59412cb9e0c5250ac1",
                "85015e85011111c26a98669f27bfcf20b424c29d4b28e7f0f07c06793899d79d",
                "aa7e6f1eb8c2913d70b4412c32b98adf2974d82b1476a90919a17f8228b81118",
                "85eb24dec3b7e40c8bc87d06ca6756f243c95a63ab262efb45f5cc8df46a33ef",
                "774c072f2521238f1f4521b797b5b1e7f77c336b1b0d62637fefec2510dfa448",
                "7537c66a553b79392f8f290b8899d9c5970395985480089cc7e04832e03c2c6b",
                "56c5996f6c014ee4ea460e101931a41e6e5dced8a164dd034f41802f16260f4d",
                "2251e53079977f09d7d6a1c9291ed440c3c911de6973ff02184f3d34afb7b585",
                "6a4515610158bf27b317d02c1ad978bf6f9d22cf6fb296bd643833bf78678a71",
                "49a31a06a61aaf2884b9d4d25face590721d9225849723821d865f8f0f7699a0",
                "af0b9a5ba01bc5b23dd7b66d56cf72f8dfa87bdbc1bb4ca260f0979b87908c34",
                "f00a71a128600d843acebee09d38d0059ab0efe6415caac082e395b8d5a45294",
                "774c072f2521238f1f4521b797b5b1e7f77c336b1b0d62637fefec2510dfa448",
                "85eb24dec3b7e40c8bc87d06ca6756f243c95a63ab262efb45f5cc8df46a33ef",
                "ea070f89bf270cce825de1fa975d4e61f4bb1ca02231cd274820396ebd762fc3",
                "d3d0cbe5041198c0695e133e82ca766928b76ede89a43cc0e0c8f106b5efa898",
                "4dcf07fc1ad313cf7b0df6bebc270538d66310d49a03460423fd2d42514a69fc",
                "badd9e5aa6112d2148be033b783a91eac5c93177fabd52e4c4b7a1ab98866a84",
                "e8e0459a7ca1a851c1555d9d627561814aa908b38a8999c2fd40dcf0b1cff8f3",
            };

            // SHA-256 dei file immagine di marchio rimossi
            var imageDigests = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "58d26a03aae7e3048187972317670cdf701f7b2ebfcf82bf539aa93adf65171d",
                "0f74a10322051e5cd2c5dd078d358f8e6a035ae566f45270d3845d46cefb3a64",
                "069aac16ad8df01e58cea70eda12bf1f8c3ede2b7c232cdd99cecdef69d4ef2d",
            };

            // Lunghezze dei marcatori, per l'hash a finestra scorrevole
            var windowLengths = new HashSet<int> { 3, 7, 9, 10, 11, 12, 15, 17, 19, 20, 21, 22, 24 };

            var root = RepositoryRoot();
            var violations = new List<string>();

            foreach (var path in AllScanFiles(root))
            {
                var extension = Path.GetExtension(path).ToLowerInvariant();
                var relative = path.Substring(root.Length + 1).Replace('\\', '/');

                if (OfficeExtensions.Contains(extension))
                {
                    foreach (var run in OfficeTextRuns(path))
                    {
                        var normalized = Normalize(run);
                        if (normalized.Length < 7) continue;

                        var limit = Math.Min(normalized.Length, 2000);

                        foreach (var length in windowLengths)
                        {
                            if (length > limit) continue;

                            var found = false;
                            for (var start = 0; start + length <= limit && !found; start++)
                            {
                                if (textDigests.Contains(Sha256(normalized.Substring(start, length))))
                                {
                                    found = true;
                                }
                            }

                            if (found)
                            {
                                violations.Add(relative + " -> marchio o dato identificativo rilevato (valore non stampato)");
                                break;
                            }
                        }
                    }
                }
                else if (ImageExtensions.Contains(extension))
                {
                    if (imageDigests.Contains(ShaFile(path)))
                    {
                        violations.Add(relative + " -> logo dell'istituto di origine (valore non stampato)");
                    }
                }
            }

            Assert.True(violations.Count == 0,
                "C1c fallito - marchio o loghi presenti:\n" + String.Join("\n", violations.Distinct()));
        }

        /// <summary>Normalizza il testo per il confronto: rimuove ogni spazio e minuscolizza.</summary>
        private static string Normalize(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (char.IsWhiteSpace(c)) continue;
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Testo dei run di un documento Office Open XML: restituisce il contenuto testuale
        /// di ogni elemento XML (fra <c>&gt;</c> e <c>&lt;</c>) delle parti word/, xl/, ppt/
        /// e docProps/.
        /// </summary>
        private static List<string> OfficeTextRuns(string path)
        {
            var result = new List<string>();

            try
            {
                using (var archive = ZipFile.OpenRead(path))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var name = entry.FullName;
                        var isTextPart =
                            (name.StartsWith("word/", StringComparison.OrdinalIgnoreCase) ||
                             name.StartsWith("xl/", StringComparison.OrdinalIgnoreCase) ||
                             name.StartsWith("ppt/", StringComparison.OrdinalIgnoreCase) ||
                             name.StartsWith("docProps/", StringComparison.OrdinalIgnoreCase)) &&
                            name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

                        if (!isTextPart) continue;

                        string content;
                        try
                        {
                            using (var reader = new StreamReader(entry.Open(), Encoding.UTF8))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        foreach (Match match in Regex.Matches(content, @">([^<>]{3,400})<"))
                        {
                            result.Add(match.Groups[1].Value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Documento non leggibile: la guardia non deve fallire per questo.
            }

            return result;
        }

        /// <summary>SHA-256 esadecimale del contenuto di un file.</summary>
        private static string ShaFile(string path)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
            {
                var bytes = sha.ComputeHash(stream);
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>Tutti i file del repository, esclusi solo gli artefatti di build e i metadati git.</summary>
        private static IEnumerable<string> AllScanFiles(string root)
        {
            var skip = new[] { ".git", "bin", "obj", "packages", "node_modules", ".vs", "TestResults" };

            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                var relative = path.Substring(root.Length + 1).Replace('\\', '/');

                if (skip.Any(d =>
                        relative.Equals(d, StringComparison.OrdinalIgnoreCase) ||
                        relative.StartsWith(d + "/", StringComparison.OrdinalIgnoreCase) ||
                        relative.Contains("/" + d + "/")))
                {
                    continue;
                }

                yield return path;
            }
        }

        /// <summary>
        /// Forme confrontabili di un token: il token stesso, la sua versione minuscola e i
        /// componenti ottenuti separando su punteggiatura e separatori. Serve a rilevare i
        /// marcatori anche quando sono immersi in un token piu' lungo (percorsi, URL, email).
        /// </summary>
        private static IEnumerable<string> Candidates(string token)
        {
            yield return token;

            var lower = token.ToLowerInvariant();
            yield return lower;

            foreach (var part in lower.Split(
                new[] { '.', ':', '@', '/', '\\', '_', '-', '+', '%' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.Length >= 3) yield return part;
            }
        }

        /// <summary>SHA-256 esadecimale (minuscolo) di una stringa UTF-8.</summary>
        private static string Sha256(string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>Tutti i file di testo del repository, esclusi binari e asset di terze parti.</summary>
        private static IEnumerable<string> AllTextFiles(string root)
        {
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

                if (ForbiddenBinaryNames.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (BinaryExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return path;
            }
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
