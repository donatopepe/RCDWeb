using System;
using System.Collections.Generic;
using System.Globalization;
using RCDWeb.Domain.Abstractions;

namespace RCDWeb.Domain.Utility
{
    /// <summary>
    /// Conversioni fra <see cref="DateTime"/> e testo nei formati usati dall'applicazione.
    /// Porting cross-platform di <c>RCDWeb.Areas.RCD.Uility.DateUtil</c>, con la dipendenza
    /// da ELMAH sostituita da <see cref="IErrorReporter"/>.
    /// </summary>
    public static class DateUtil
    {
        /// <summary>Numero massimo di anni indietro accettati per una data.</summary>
        public const int MaxYearsBack = 120;

        /// <summary>
        /// Formati accettati in input, nell'ordine di tentativo.
        /// </summary>
        public static readonly IReadOnlyList<string> AcceptedFormats = new List<string>
        {
            "ddMMyy",
            "ddMMyyyy",
            "dd/MM/yyyy",
            "dd/MM/yy",
            "dd/MM/yyyy H.mm.ss",
            "dd/MM/yyyy HH.mm.ss",
        }.AsReadOnly();

        /// <summary>
        /// Converte una data in testo.
        /// Restituisce stringa vuota se la data è nulla o se precede di oltre
        /// <see cref="MaxYearsBack"/> anni la data corrente (errore segnalato al reporter).
        /// </summary>
        public static string DataToText(DateTime? value, bool viewerror = true, string format = "dd/MM/yyyy")
        {
            if (value == null) return String.Empty;

            var localDate = DateTime.Now;
            if (value < localDate.AddYears(-MaxYearsBack))
            {
                if (viewerror) ErrorReporter.Report(new Exception("Errore Data"));
                return String.Empty;
            }

            return ((DateTime)value).ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converte testo in data, provando i formati di <see cref="AcceptedFormats"/>,
        /// poi la cultura it-IT e infine en-EN.
        /// Restituisce <c>null</c> se nessun formato è applicabile o se la data precede
        /// di oltre <see cref="MaxYearsBack"/> anni la data corrente (errore segnalato al reporter).
        /// </summary>
        public static DateTime? TextToDate(string value, bool viewerror = true)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;

            DateTime result = DateTime.MinValue;
            var dataOK = false;

            foreach (var item in AcceptedFormats)
            {
                dataOK = DateTime.TryParseExact(
                    value, item, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                if (dataOK) break;
            }

            if (!dataOK)
            {
                dataOK = TryParseCulture(value, "it-IT", out result);

                if (!dataOK)
                {
                    dataOK = TryParseCulture(value, "en-EN", out result);

                    if (!dataOK)
                    {
                        if (viewerror)
                            ErrorReporter.Report(new Exception("Errore formato Data:" + value));
                        return null;
                    }
                }
            }

            var localDate = DateTime.Now;
            if (result < localDate.AddYears(-MaxYearsBack))
            {
                if (viewerror)
                    ErrorReporter.Report(new Exception("Errore Data:" + value));
                return null;
            }

            return result;
        }

        private static bool TryParseCulture(string value, string culture, out DateTime result)
        {
            try
            {
                return DateTime.TryParse(
                    value, CultureInfo.GetCultureInfo(culture), DateTimeStyles.None, out result);
            }
            catch (CultureNotFoundException)
            {
                // Cultura non disponibile nell'host: il formato è semplicemente non applicabile.
                result = DateTime.MinValue;
                return false;
            }
        }
    }
}
