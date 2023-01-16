using Elmah;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RCDWeb.Areas.RCD.Uility
{
    public static class DateUtil
    {

        public static string DataToText(DateTime? value, bool viewerror = true, string format = "dd/MM/yyyy")
        {
            if (value == null) return String.Empty;
            DateTime localDate = DateTime.Now;
            if (value < localDate.AddYears(-120))
            {
                if (viewerror) ErrorSignal.FromCurrentContext().Raise(new Exception("Errore Data"));

                return String.Empty;
            }
            return ((DateTime)value).ToString(format, CultureInfo.InvariantCulture);
        }

        private static List<string> FormatoDate = new List<string>()
        {
            "ddMMyy",
            "ddMMyyyy",
            "dd/MM/yyyy",
            "dd/MM/yy",
            "dd/MM/yyyy H.mm.ss",
            "dd/MM/yyyy HH.mm.ss",
        };
        public static DateTime? TextToDate(string value, bool viewerror = true)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            DateTime result = DateTime.MinValue;
            var dataOK = false;
            foreach (var item in FormatoDate)
            {
                dataOK = DateTime.TryParseExact(value, item, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                if (dataOK) break;
            }

            if (!dataOK)
            {
                dataOK = DateTime.TryParse(value, CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.None, out result);
                if (!dataOK)
                {
                    dataOK = DateTime.TryParse(value, CultureInfo.GetCultureInfo("en-EN"), DateTimeStyles.None, out result);
                    if (!dataOK)
                    {
                        if (viewerror) ErrorSignal.FromCurrentContext().Raise(new Exception("Errore formato Data:" + value));
                        return null;
                    }
                }
            }

            if (dataOK)
            {
                DateTime localDate = DateTime.Now;
                if (result < localDate.AddYears(-120))
                {
                    if (viewerror) ErrorSignal.FromCurrentContext().Raise(new Exception("Errore Data:" + value));
                    return null;
                }
            }
            return result;
        }



        //public string DataToText(DateTime? value)
        //{
        //    if (value == null) return String.Empty;
        //    DateTime localDate = DateTime.Now;
        //    if (value < localDate.AddYears(-120))
        //    {
        //        return String.Empty;
        //    }
        //    return ((DateTime)value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //}
        //public DateTime? TextToDate(string value)
        //{
        //    if (String.IsNullOrWhiteSpace(value))
        //    {
        //        return null;
        //    }
        //    DateTime result;
        //    var dataOK = DateTime.TryParseExact(value, "ddMMyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        //    if (!dataOK)
        //    {
        //        dataOK = DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        //        if (!dataOK)
        //        {
        //            dataOK = DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        //            if (!dataOK)
        //            {
        //                dataOK = DateTime.TryParseExact(value, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        //                if (!dataOK)
        //                {
        //                    dataOK = DateTime.TryParse(value, CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.None, out result);
        //                    if (!dataOK)
        //                    {
        //                        return null;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    if (dataOK)
        //    {
        //        DateTime localDate = DateTime.Now;
        //        if (result < localDate.AddYears(-120))
        //        {
        //            return null;
        //        }
        //    }
        //    return result;
        //}
    }
}