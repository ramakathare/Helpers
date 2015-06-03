using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.PO.SP.CaseMgmnt.Helper
{
    public static class DataTypeHelpers
    {
        public static Int32 ToInt32C(this object obj, Int32 defaultValue = 0)
        {
            if (obj == null) return defaultValue;
            try
            {
                return Convert.ToInt32(obj);
            }
            catch { }
            return defaultValue;
        }
        public static string ToStringC(this object obj, string defaultValue = "")
        {
            try
            {
                return obj==null ? defaultValue : Convert.ToString(obj);
            }
            catch { }
            return defaultValue;
        }
        public static bool IsUndefined(this object obj)
        {
            try
            {
                return (string.IsNullOrEmpty(Convert.ToString(obj)) || string.IsNullOrWhiteSpace(Convert.ToString(obj)));
            }
            catch { }
            return true;
        }
    }
}
