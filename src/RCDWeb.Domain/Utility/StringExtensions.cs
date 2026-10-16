using System;

namespace RCDWeb.Domain.Utility
{
    /// <summary>
    /// Estensioni di confronto su stringhe che ignorano spazi e differenze di maiuscole.
    /// Porting cross-platform di <c>RCDWeb.Areas.RCD.Uility.Extensions</c>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Confronta due stringhe dopo aver rimosso tutti gli spazi e aver convertito
        /// i caratteri in minuscolo.
        /// </summary>
        /// <remarks>
        /// Il comportamento è quello storico: la conversione in minuscolo non è invariant
        /// per cultura, quindi il risultato può dipendere dalla cultura corrente
        /// (es. la 'I' turca). Mantenuto per compatibilità.
        /// </remarks>
        public static bool EqualsNormalized(this string a, string b)
        {
            if (!String.IsNullOrWhiteSpace(a)) a = a.Replace(" ", "").ToLower();
            if (!String.IsNullOrWhiteSpace(b)) b = b.Replace(" ", "").ToLower();
            return a == b;
        }

        /// <summary>
        /// Verifica se <paramref name="a"/> contiene <paramref name="b"/> dopo aver rimosso
        /// tutti gli spazi e aver convertito i caratteri in minuscolo.
        /// </summary>
        public static bool ContainsNormalized(this string a, string b)
        {
            if (!String.IsNullOrWhiteSpace(a)) a = a.Replace(" ", "").ToLower();
            if (!String.IsNullOrWhiteSpace(b)) b = b.Replace(" ", "").ToLower();
            return a != null && a.Contains(b ?? String.Empty);
        }
    }
}
