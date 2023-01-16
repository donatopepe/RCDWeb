using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RCDWeb.Areas.RCD.Uility
{
    public class DateUtil
    {
        public string DataToText(DateTime? value)
        {
            if (value == null) return String.Empty;
            DateTime localDate = DateTime.Now;
            if (value < localDate.AddYears(-120))
            {
                return String.Empty;
            }
            return ((DateTime)value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        public DateTime? TextToDate(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            DateTime result;
            var dataOK = DateTime.TryParseExact(value, "ddMMyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            if (!dataOK)
            {
                dataOK = DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                if (!dataOK)
                {
                    dataOK = DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                    if (!dataOK)
                    {
                        dataOK = DateTime.TryParseExact(value, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
                        if (!dataOK)
                        {
                            return null;
                        }
                    }
                }

            }
            if (dataOK)
            {
                DateTime localDate = DateTime.Now;
                if (result < localDate.AddYears(-120))
                {
                    return null;
                }
            }
            return result;
        }
    }
}