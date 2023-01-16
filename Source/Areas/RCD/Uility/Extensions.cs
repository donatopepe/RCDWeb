using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCDWeb.Areas.RCD.Uility
{
    public static class Extensions
    {
        public static bool EqualsNormalized(this string a, string b)
        {
            if (!String.IsNullOrWhiteSpace(a)) a = a.Replace(" ", "").ToLower();
            if (!String.IsNullOrWhiteSpace(b)) b = b.Replace(" ", "").ToLower();
            return a == b;
        }
        public static bool ContainsNormalized(this string a, string b)
        {
            if (!String.IsNullOrWhiteSpace(a)) a = a.Replace(" ", "").ToLower();
            if (!String.IsNullOrWhiteSpace(b)) b = b.Replace(" ", "").ToLower();
            return a.Contains(b);
        }
    }
}