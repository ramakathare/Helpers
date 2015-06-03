using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Global.Helpers
{
    public class JsonResult
    {
        public JsonResult(bool _success, string _message, object _data = null)
        {
            this.success = _success;
            this.message = _message;
            this.data = _data;
        }
        public JsonResult(Exception ex)
        {
            this.success = false;
            if (ex.ToString().IndexOf("conflicted with the REFERENCE constraint") > 0)
                this.message = "This record is being referenced in other places, hence can not be deleted";
            else
                this.message = ex.Message;
        }
        public bool success { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}
