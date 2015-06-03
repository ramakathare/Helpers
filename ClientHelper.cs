using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace TT.PO.SP.CaseMgmnt.Helper
{
    public static class ClientHelperExtension
    {
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
    public class ClientHelper
    {
        public string category { get; set; }
        public ClientHelper(string _Category)
        {
            if (String.IsNullOrEmpty(_Category)) category = "CustomCat"; else category = _Category;
        }
        public void alert(string message, Page page)
        {
            string scriptText = "alert('" + message + "');";
            ScriptManager.RegisterStartupScript(page, this.GetType(), category, scriptText, true);
        }
        public void elog(string message, Page page)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                var cs = category;
                if (!EventLog.SourceExists(cs))
                    EventLog.CreateEventSource(cs, "Application");
                EventLog.WriteEntry(cs, message, EventLogEntryType.Information);
            });
        }
        public void clog(string message, Page page)
        {
            string scriptText = "console.log('" + message + "');";
            ScriptManager.RegisterStartupScript(page, this.GetType(), "foo", scriptText, true);
        }
        public void clog(string message,Exception ex, Page page)
        {
            string info = ex.Message;
            info += ex.StackTrace;
            if (ex.InnerException != null)
            {
                info += "\n" + ex.InnerException.Message;
                info += ex.InnerException.StackTrace;
                if (ex.InnerException.InnerException != null)
                {
                    info += "\n" + ex.InnerException.InnerException.Message;
                    info += ex.InnerException.InnerException.StackTrace;
                }
            }
            clog(info, page);
        }
        public void UpdateULSLogging(Exception ex, EventSeverity EventType, bool ToULSLog = true, bool ToEventViewer = false)
        {
            string info = ex.Message + "\n" + ex.StackTrace.ToStringC();
            if (ex.InnerException != null)
            {
                info += "\n" + ex.InnerException.Message + "\n" + ex.InnerException.StackTrace.ToStringC();
                if (ex.InnerException.InnerException != null)
                {
                    info += "\n" + ex.InnerException.InnerException.Message + "\n" + ex.InnerException.InnerException.StackTrace.ToStringC();
                }
            }
            UpdateULSLogging(info, EventType, ToULSLog, ToEventViewer);
        }
        public void UpdateULSLogging(string infoStr, EventSeverity EventType,bool ToULSLog = true, bool ToEventViewer = false)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (ToULSLog)
                {
                    SPDiagnosticsService spTransmitalLogSvc = SPDiagnosticsService.Local;
                    foreach (var item in infoStr.SplitByLength(500))
                    {
                        spTransmitalLogSvc.WriteTrace(0, new SPDiagnosticsCategory(category, TraceSeverity.Monitorable, EventType),
                           TraceSeverity.Monitorable, "Writing to the ULS log : {0}", new object[] { item });
                    }
                }
                if (ToEventViewer)
                {
                    string sSource = "IncidentManagement";
                    string sLog = "Application";
                    string sEvent = infoStr;

                    if (!EventLog.SourceExists(sSource))
                        EventLog.CreateEventSource(sSource, sLog);

                    EventLog.WriteEntry(sSource, sEvent);
                    EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Information, 459);
                }
            });
        }
        public void RedirectToList(SPWeb sWeb, Page page, String ListName)
        {
            SPList sList = sWeb.Lists[ListName];
            string listUrl = sList.DefaultViewUrl;
            page.Response.Redirect(listUrl);
        }
        
    }
    
}
