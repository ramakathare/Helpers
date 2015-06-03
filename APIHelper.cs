using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TT.PO.SP.CaseMgmnt.Helper
{
    public static class APIHelper
    {
        public static string SaveUpdate(WebClient client,string baseAPIUrl, string controller, string data, bool isEdit)
        {
            var httpVerb = "POST";
            baseAPIUrl = baseAPIUrl + controller + "/";
         //   if (isEdit)
         //   {
         //       httpVerb = "PUT";
         //       baseAPIUrl = baseAPIUrl + ID;
         //   }
            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Accept", "application/json");
            return client.UploadString(baseAPIUrl, httpVerb, data);
        }          
    }
}
