using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TT.Global.Helpers
{
    public static class Extensions
    {
        public static string Serialize(this Object o)
        {
            return new JavaScriptSerializer().Serialize(o);
        }
        public static T DeSerialize<T>(this string s)
        {
            return new JavaScriptSerializer().Deserialize<T>(s);
        }
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
}
