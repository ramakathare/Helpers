using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TT.PO.SP.CaseMgmnt.Helper.Objects
{
    public partial class Case : ItemModel
    {
        public Case() { }

        //The following constructor is like an auto mapper from SPListItem to this class
        public Case(SPListItem item, SPWeb sWeb) : base()
        {
            ID = item.GetValue<int>("ID");
            Title = item.GetValue<string>(CaseC.Title);
            TTPO_ActionsArray = item.GetValue<string>(CaseC.TTPO_ActionsArray);
            TTPO_CaseFilesInfo = item.GetValue<string>(CaseC.TTPO_CaseFilesInfo);
            TTPO_ResolutionFilesInfo = item.GetValue<string>(CaseC.TTPO_ResolutionFilesInfo);
            TTPO_CaseDate = item.GetValue<DateTime>(CaseC.TTPO_CaseDate);
            TTPO_CaseDescriptionBrief = item.GetValue<string>(CaseC.TTPO_CaseDescriptionBrief);
            TTPO_CaseDescriptionDetailed = item.GetValue<string>(CaseC.TTPO_CaseDescriptionDetailed);
            TTPO_CaseNumber = item.GetValue<string>(CaseC.TTPO_CaseNumber);
            TTPO_CaseOwner = item.GetSPUser(CaseC.TTPO_CaseOwner,sWeb);
            TTPO_CaseStatusBrief = item.GetValue<string>(CaseC.TTPO_CaseStatusBrief);
            TTPO_CaseType = item.GetValue<string>(CaseC.TTPO_CaseType);
            TTPO_CasePriority = item.GetValue<string>(CaseC.TTPO_CasePriority);
            TTPO_EnteredBy = item.GetSPUser(CaseC.TTPO_EnteredBy, sWeb);
            TTPO_FinalResolution = item.GetValue<string>(CaseC.TTPO_FinalResolution);
            TTPO_PlannedDate = item.GetValue<DateTime>(CaseC.TTPO_PlannedDate);
            TTPO_ProposedSolution = item.GetValue<string>(CaseC.TTPO_ProposedSolution);
            TTPO_ResolutionDate = item.GetValue<DateTime>(CaseC.TTPO_ResolutionDate);
            TTPO_StakeholderID = item.GetValue<string>(CaseC.TTPO_StakeholderID);
            TTPO_StakeholderAcceptance = item.GetValue<string>(CaseC.TTPO_StakeholderAcceptance);
            TTPO_StakeholderComments = item.GetValue<string>(CaseC.TTPO_StakeholderComments);
            TTPO_SuspendOwnerAllocation = item.GetValue<bool>(CaseC.TTPO_SuspendOwnerAllocation);
            TTPO_ReallocatedCaseOwner = item.GetSPUser(CaseC.TTPO_ReallocatedCaseOwner,sWeb);
            TTPO_IssueToCaseOwnerForResolution = item.GetValue<bool>(CaseC.TTPO_IssueToCaseOwnerForResolution);
            TTPO_IssueToCaseReallocatedOwnerForResolution = item.GetValue<bool>(CaseC.TTPO_IssueToCaseReallocatedOwnerForResolution);
            TTPO_CaseCloseDate = item.GetValue<DateTime>(CaseC.TTPO_CaseCloseDate);
        }
        public int ID { get; set; }
        public string Title { get; set; }
        public string TTPO_ActionsArray { get; set; }
        public DateTime TTPO_CaseDate { get; set; }
        public string TTPO_CaseDescriptionBrief { get; set; }
        public string TTPO_CaseDescriptionDetailed { get; set; }
        public string TTPO_CaseNumber { get; set; }
        [ScriptIgnore]
        public SPUser TTPO_CaseOwner { get; set; }
        public string TTPO_CaseStatusBrief { get; set; }
        public string TTPO_CaseType { get; set; }
        public string TTPO_CasePriority { get; set; }
        [ScriptIgnore]
        public SPUser TTPO_EnteredBy { get; set; }
        public string TTPO_FinalResolution { get; set; }
        public DateTime TTPO_PlannedDate { get; set; }
        public string TTPO_ProposedSolution { get; set; }
        public DateTime TTPO_ResolutionDate { get; set; }
        public string TTPO_StakeholderID { get; set; }
        public string TTPO_StakeholderAcceptance { get; set; }
        public string TTPO_StakeholderComments { get; set; }
        public bool TTPO_SuspendOwnerAllocation { get; set; }
        public string TTPO_CaseFilesInfo { get; set; }
        public string TTPO_ResolutionFilesInfo { get; set; }
        public string TTPO_CaseFilesInfoAttachments { get; set; }
        public string TTPO_ResolutionFilesInfoAttachments { get; set; }
        [ScriptIgnore]
        public SPUser TTPO_ReallocatedCaseOwner { get; set; }
        public bool TTPO_IssueToCaseOwnerForResolution{ get; set; }
        public bool TTPO_IssueToCaseReallocatedOwnerForResolution { get; set; }
        public DateTime TTPO_CaseCloseDate { get; set; }
    }
    public partial class Case
    {
        //Since SPUser can not be serialized, the loginname is assigned to the below property and '_FFFF_' will be removed before passing to api
        public string TTPO_EnteredBy_FFFF_ { get { return TTPO_EnteredBy.LoginName; } set { } }
    }
}
