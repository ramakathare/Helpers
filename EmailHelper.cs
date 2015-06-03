using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TT.PO.SP.CaseMgmnt.Helper.Objects;

namespace TT.PO.SP.CaseMgmnt.Helper
{
    public static class EmailHelper
    {
        public static byte[] SendEmailToCaseOwners(Constants constants, SmtpClient server, SPWeb sWeb, Case cCase, string emailKey, string subject, string To, string cc, ref bool isDelivered)
        {
            EmailTemplate et = EmailHelper.GetEmailTemplate(sWeb, constants.EmailTemplate, constants.CaseEmailTemplateTargetList, emailKey);
            et.CC = et.CC.IsUndefined() ? cc.IsUndefined() ? null : cc : cc.IsUndefined() ? et.CC : et.CC + ";" + cc;
            et.To = et.To.IsUndefined() ? To.IsUndefined() ? null : To : To.IsUndefined() ? et.To : et.To + ";" + To;
            if (!et.Subject.IsUndefined())
            {
                et.Subject = et.Subject.Replace("[CaseNumber]", cCase.TTPO_CaseNumber);
                et.Subject = et.Subject.Replace("[Subject]", subject);
            }
            if (!et.Body.IsUndefined())
            {
                et.Body = et.Body.Replace("[clickhere]", "<a href='" + sWeb.Url + "/Lists/CaseManagement/DispForm.aspx?ID=" + cCase.ID + "'>click here</a>");
                //et.Body = et.Body.Replace("[clickhere]", "<a href='" + sWeb.Url + sWeb.Lists.TryGetList(Constants.CaseManagement).DefaultDisplayFormUrl +"?ID=" + cCase.ID + "'>click here</a>");
                et.Body = et.Body.Replace("[CustomerName]", "");
                et.Body = et.Body.Replace("[InitiatedBy]", cCase.TTPO_EnteredBy.Email);
                et.Body = et.Body.Replace("[CaseNumber]", cCase.TTPO_CaseNumber);
                et.Body = et.Body.Replace("[CaseStatus]", cCase.TTPO_CaseStatusBrief);
            }
            return SendEmail(server, sWeb, et, ref isDelivered);
        }
        public static byte[] SendEmailToStakeHolder(Constants constants, SmtpClient server, SPWeb sWeb, Case cCase, StakeHolder stakeholder, string emailTemplate, string subject, string To, string cc, ref bool isDelivered)
        {
            EmailTemplate et = EmailHelper.GetEmailTemplate(sWeb, constants.EmailTemplate, constants.CaseEmailTemplateTargetList, emailTemplate);
            et.CC = et.CC.IsUndefined() ? cc.IsUndefined() ? null : cc : cc.IsUndefined() ? et.CC : et.CC + ";" + cc;
            et.To = et.To.IsUndefined() ? To.IsUndefined() ? null : To : To.IsUndefined() ? et.To : et.To + ";" + To;
            if (!et.Subject.IsUndefined())
            {
                et.Subject = et.Subject.Replace("[CaseNumber]", cCase.TTPO_CaseNumber);
                et.Subject = et.Subject.Replace("[Subject]", subject);
            }
            if (!et.Body.IsUndefined())
            {
                et.Body = et.Body.Replace("[clickhere]", "<a href='" + sWeb.Url + "/Lists/CaseManagement/DispForm.aspx?ID=" + cCase.ID + "'>click here</a>");
                et.Body = et.Body.Replace("[CustomerName]",stakeholder.TTPO_StakeholderSalutation + " " + stakeholder.TTPO_StakeholderFirstName + " " + stakeholder.TTPO_StakeholderSurname);
                et.Body = et.Body.Replace("[InitiatedBy]", cCase.TTPO_EnteredBy.Email);
                et.Body = et.Body.Replace("[CaseNumber]", cCase.TTPO_CaseNumber);
                et.Body = et.Body.Replace("[CaseStatus]", cCase.TTPO_CaseStatusBrief);
            }
            return SendEmail(server, sWeb, et, ref isDelivered);
        }
        public static byte[] SendEmail(SmtpClient server, SPWeb web, EmailTemplate et, ref bool isDelivered)
        {
            using (MailMessage msg = new MailMessage())
            {

                try
                {
                    msg.From = new MailAddress(web.Site.WebApplication.OutboundMailSenderAddress, et.From);

                    if (!et.To.IsUndefined())
                    {
                        et.To = ReplaceJunk(et.To).Replace(";", ",");
                        msg.To.Add(et.To);
                    }
                    if (!et.CC.IsUndefined())
                    {
                        et.CC = ReplaceJunk(et.CC).Replace(";", ",");
                        msg.CC.Add(et.CC);
                    }
                    if (!et.BCC.IsUndefined())
                    {
                        et.BCC = ReplaceJunk(et.To).Replace(";", ",");
                        msg.Bcc.Add(et.BCC);
                    }

                    msg.Subject = et.Subject;
                    msg.Body = et.Body;
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    //server.SendCompleted += server_SendCompleted;
                    server.Send(msg);
                    //server.SendAsync(msg,new SendAsyncState(new EmailMessageInfo(){
                    //    RecipientName = et.To
                    //}));
                    isDelivered = true;
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    string result = "";
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        result += status + " : " + ex.InnerExceptions[i].Message + "\n";
                    }
                    new ClientHelper("wp_casemgmt_logEmail").UpdateULSLogging(ex, (EventSeverity)(EventSeverity.Error));
                }
                catch (Exception ex)
                {
                    new ClientHelper("wp_casemgmt_logEmail").UpdateULSLogging(ex, (EventSeverity)(EventSeverity.Error));
                }
                return msg.GetAsMemoryStream();
            }
        }

        


        private static byte[] GetAsMemoryStream(this MailMessage message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Assembly assembly = typeof(SmtpClient).Assembly;
                Type mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");
                ConstructorInfo mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);
                object mailWriter = mailWriterContructor.Invoke(new object[] { stream });
                MethodInfo sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);
                ParameterInfo[] api = sendMethod.GetParameters();
                if (api.Length == 2) sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true }, null);
                else sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);
                MethodInfo closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
                return stream.ToArray();
            }
        }
        
        internal static string ReplaceJunk(string to)
        {

            string[] myArray = to.Split(';');
            string final = string.Empty;
            foreach (string nam in myArray)
            {
                if (!string.IsNullOrEmpty(nam))
                {
                    final = string.Concat(final, ',', nam);
                }
            }
            return final.Trim(',');
        }
        internal static string InternalNotificationList(SPWeb objSPweb)
        {
            string strusermails = string.Empty;
            SPGroup oGroup = objSPweb.SiteGroups["NCRInternalNotifications"];
            if (oGroup != null)
            {
                foreach (SPUser objspuser in oGroup.Users)
                {
                    if (!string.IsNullOrEmpty(objspuser.Email))
                    {
                        strusermails = string.Concat(strusermails, ',', objspuser.Email);
                    }
                }
            }
            return strusermails.Trim(',');
        }
        public static EmailTemplate GetEmailTemplate(SPWeb web, string listName, string TargetList, string emailKey)
        {
            EmailTemplate e = new EmailTemplate();
            SPList list = web.Lists.TryGetList(listName);
            if (list != null)
            {
                SPQuery qry = new SPQuery();
                qry.RowLimit = 1;
                qry.Query = @"   <Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>" + emailKey + "</Value></Eq><Eq><FieldRef Name='TTPOTargetList' /><Value Type='Text'>" + TargetList + "</Value></Eq></And></Where>";
                qry.ViewFields = @"<FieldRef Name='TTPOEmailBody' /><FieldRef Name='TTPOEmailSubject' /><FieldRef Name='TTPOTo' /><FieldRef Name='TTPOCC' /><FieldRef Name='TTPOBCC' /><FieldRef Name='TTPOEmailFrom' />";
                SPListItemCollection listItems = list.GetItems(qry);
                //SPFieldMultiLineText mailBody = listItems[0].Fields["TTPOEmailBody"] as SPFieldMultiLineText;
                SPFieldMultiLineText mailBody = listItems[0].Fields.GetFieldByInternalName("TTPOEmailBody") as SPFieldMultiLineText;
                e.Body = mailBody.GetFieldValueAsHtml(listItems[0].GetValue<string>("TTPOEmailBody"));
                e.From = listItems[0].GetValue<string>("TTPOEmailFrom");
                e.To = SPHelper.GetEmailsFromSPUserCollection(listItems[0], "TTPOTo");
                e.CC = SPHelper.GetEmailsFromSPUserCollection(listItems[0], "TTPOCC");
                e.BCC = SPHelper.GetEmailsFromSPUserCollection(listItems[0], "TTPOBCC");
                e.Subject = listItems[0].GetValue<string>("TTPOEmailSubject");
            }
            return e;
        }

        //static void server_SendCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    var smtpClient = (SmtpClient)sender;
        //    var userAsyncState = (SendAsyncState)e.UserState;
        //    smtpClient.SendCompleted -= server_SendCompleted;

        //    //if(e.Error != null) {
        //    //   tracer.ErrorEx(
        //    //      e.Error, 
        //    //      string.Format("Message sending for \"{0}\" failed.",userAsyncState.EmailMessageInfo.RecipientName)
        //    //   );
        //    //}
        //    //Clean resources
        //}
        //public class EmailMessageInfo
        //{
        //    public string RecipientName { get; set; }
        //}
        //public class SendAsyncState
        //{

        //    /// <summary>
        //    /// Contains all info that you need while handling message result
        //    /// </summary>
        //    public EmailMessageInfo EmailMessageInfo { get; private set; }


        //    public SendAsyncState(EmailMessageInfo emailMessageInfo)
        //    {
        //        EmailMessageInfo = emailMessageInfo;
        //    }
        //}
    }
}
