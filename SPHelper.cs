using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.PO.SP.CaseMgmnt.Helper.Objects;

namespace TT.PO.SP.CaseMgmnt.Helper
{
    public static class SPHelper
    {
        public static List<CustomFile> MoveUploadedFileNames(this SPWeb sWeb, string FolderName, string DocLibraryName,string DocLibraryTitle, string prefixText, string uploadedFiles, string subFolder = null)
        {
            List<CustomFile> AddedFilesInfo = new List<CustomFile>();
            if (!string.IsNullOrWhiteSpace(uploadedFiles))
            {
                string[] seperator = new string[] { ";" };
                string[] uploadedFileList = uploadedFiles.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                if (uploadedFileList.Length > 0)
                {
                    sWeb.AllowUnsafeUpdates = true;
                    string folderRootUrl = string.Format("{0}/{1}", sWeb.Url, DocLibraryTitle);
                    string uploadedFileUrl = string.Empty;
                    SPFolder sFolderRoot = sWeb.GetFolder(folderRootUrl);

                    if (sFolderRoot != null)
                    {
                        SPFolder sFolder = sWeb.CreateFolderStructure(DocLibraryName, FolderName);
                        if (subFolder != null) sFolder = sWeb.CreateFolderStructure(DocLibraryName, FolderName + "/" + subFolder);

                        if (sFolder != null)
                        {
                            Dictionary<string, string> fileList = new Dictionary<string, string>();
                            string fileNameOriginal = string.Empty;

                            foreach (string file in uploadedFileList)
                            {
                                fileNameOriginal = file.Replace(prefixText, "");

                                //uploadedFileUrl = string.Format("{0}/{1}/{2}/{3}", sWeb.Url, DocLibraryName, sFolder.Name, file);
                                string movedFileUrl = string.Format("{0}/{1}/{2}", sWeb.Url, sFolder.Url, fileNameOriginal);


                                SPFile sFile = sWeb.GetFile(sFolderRoot.Url + "/" + file);
                                sFile.MoveTo(movedFileUrl, SPMoveOperations.Overwrite);
                                AddedFilesInfo.Add(new CustomFile(fileNameOriginal, movedFileUrl, sFile.Name.Substring(sFile.Name.LastIndexOf(".") + 1), sFile.Length));
                            }
                        }
                    }
                    sWeb.AllowUnsafeUpdates = false;
                }
            }
            return AddedFilesInfo;
        }
        public static List<CustomFile> DeleteUploadedFiles(this SPWeb sWeb, string deleteFiles, string call="B")
        {
            List<CustomFile> deletedFilesInfo = new List<CustomFile>();
            if (!string.IsNullOrWhiteSpace(deleteFiles))
            {
                string[] seperator = new string[] { ";" };
                string[] uploadedFileList = deleteFiles.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                sWeb.AllowUnsafeUpdates = true;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    if (uploadedFileList.Length > 0)
                    {
                        foreach (string file in uploadedFileList)
                        {
                            SPFile sFile = sWeb.GetFile(file);
                            var name = sFile.Name;
                            var extension = sFile.Name.Substring(sFile.Name.LastIndexOf(".") + 1);
                            var length = sFile.Length;
                            sFile.Delete();

                            deletedFilesInfo.Add(new CustomFile(name, file, extension, length));
                        }
                    }
                });
                sWeb.AllowUnsafeUpdates = false;
            }
            return deletedFilesInfo;
        }
        public static void UpdateSingleColumn(this SPWeb sWeb, string ListName, int ItemID, string Field, string Value)
        {
            //SPList sList = sWeb.Lists[ListName];
            SPList sList = sWeb.Lists.TryGetList(ListName);

            if (sList != null)
            {
                SPListItem sListItem = null;

                if (ItemID > 0)
                {
                    sListItem = sList.GetItemById(ItemID);
                }

                if (sListItem != null)
                {
                    sListItem[Field] = Value;
                    sListItem.SystemUpdate();
                }
            }
        }
        public static SPFolder CreateFolderStructure(this SPWeb sWeb, string listName, string forlderStructure)
        {
            SPFolder currentFolder = null;

            var folderPaths = forlderStructure.Split('/');

            SPList sList = sWeb.Lists[listName];
            currentFolder = sList.RootFolder;

            for (int i = 0; i < folderPaths.Length; i++)
            {
                if (!string.IsNullOrEmpty(folderPaths[i].Trim()))
                {
                    currentFolder = sList.ParentWeb.GetFolder(currentFolder.Url);
                    var subFolder = currentFolder.SubFolders.Cast<SPFolder>().FirstOrDefault(f => string.Compare(f.Name, folderPaths[i], true) == 0);
                    if (subFolder == null)
                    {
                        var newFolderItem = sList.Items.Add(currentFolder.ServerRelativeUrl, SPFileSystemObjectType.Folder, folderPaths[i]);
                        newFolderItem.SystemUpdate();

                        subFolder = newFolderItem.Folder;
                    }
                    currentFolder = subFolder;
                }
            }

            return currentFolder;
        }
        public static T GetSettings<T>(this SPWeb sWeb, string Key,string ListName = "SiteSettings", string KeyField = "Title", string ValueField = "Value")
        {
            SPList sList = sWeb.Lists[ListName];
            var FilterQuery = "<Where>" +
                                    "<Eq>" +
                                        "<FieldRef Name='" + KeyField + "'/>" +
                                        "<Value Type='Text'>" + Key + "</Value>" +
                                    "</Eq>" +
                                 "</Where>";
            SPQuery spQuery = new SPQuery();
            spQuery.Query = string.Concat(FilterQuery,"<OrderBy><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>");
            spQuery.ViewFields = string.Concat("<FieldRef Name='" + ValueField + "' />");
            spQuery.RowLimit = 1;

            SPListItemCollection sListItems = sList.GetItems(spQuery);
            if ((sListItems != null) && (sListItems.Count > 0))
            {
                object value = sListItems[0][ValueField];
                if (value == null) return default(T);
                var t = typeof(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable)))
                {
                    if (value == null || value == "") return default(T);
                    t = Nullable.GetUnderlyingType(t);
                }
                return (T)Convert.ChangeType(value, t);
            }
            else
            {
                return default(T);
            }
        }
        
        public static string GetLastReference(this SPWeb sWeb, string ListName, string fieldName, string paddingString, int paddingLength, string subQuery = "")
        {
            SPList sList = sWeb.Lists[ListName];

            string newCMReference = string.Empty;

            SPQuery spQuery = new SPQuery();
            spQuery.Query = string.Concat(
                                    subQuery,"<OrderBy><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>"
                                );

            spQuery.ViewFields = string.Concat(
                                    "<FieldRef Name='" + fieldName + "' />"
                                );
            spQuery.RowLimit = 1;

            SPListItemCollection sListItems = sList.GetItems(spQuery);
            if ((sListItems != null) && (sListItems.Count > 0))
            {
                foreach (SPListItem sListItem in sListItems)
                {
                    int lastReferenceNumber = fieldName == null ? SPHelper.GetValue<int>(sListItem, "ID") : sListItem[fieldName].ToStringC("0").Replace(paddingString, "").ToInt32C();
                    int newReferenceNumber = lastReferenceNumber + 1;
                    newCMReference = paddingString + newReferenceNumber.ToString().PadLeft(paddingLength, '0');
                    break;
                }
            }
            else
            {
                int newRFINo = 1;
                newCMReference = paddingString + newRFINo.ToString().PadLeft(paddingLength, '0');

            }
            return newCMReference;
        }
        public static int SaveUpdateListItem(this SPWeb sWeb, string ListName, ItemModel item, FieldInfo[] Fields)
        {
            SPList sList = sWeb.Lists[ListName];
            //FieldInfo[] Fields = ConstantsC.GetFields();
            if (sList != null)
            {
                SPListItem sListItem = null;
                if (item.ID > 0)
                    sListItem = sList.GetItemById(item.ID);
                else
                    sListItem = sList.AddItem();

                foreach (var field in Fields)
                {
                    if (field.IsLiteral)
                        sListItem[field.Name] = item.GetType().GetProperty(field.Name).GetValue(item, null);
                }

                sListItem.Update();
                return sListItem.ID;
            }
            else throw new Exception("There is no List under the name " + ListName);
        }
        public static SPUser GetUserFromPeoplePicker(this SPWeb web, PeopleEditor people)
        {
            List<string> values = new List<string>();
            if (people.ResolvedEntities.Count > 0)
            {
                for (int i = 0; i < people.ResolvedEntities.Count; i++)
                {
                    PickerEntity user = (PickerEntity)people.ResolvedEntities[i];
                    SPUser webUser = web.EnsureUser(user.Key);
                    return webUser;
                }
            }
            return null;
        }
        //The following function is not tested yet
        //public static T GetObject<T>(this SPWeb sWeb, string ListName,Hashtable Filter)
        //{
        //    SPList sList = sWeb.Lists[ListName];
        //    SPListItem item = null;
        //    if (sList != null)
        //    {
        //        SPQuery oQuery = new SPQuery();
        //        oQuery.Query = @"<Where>";
        //        foreach(DictionaryEntry entry in Filter){
        //            oQuery.Query += @"<FieldRef Name='" + entry.Key + "' Value='" + entry.Value + "'/>";
        //        }
        //        oQuery.Query += "</Where>";
        //        oQuery.Query += @"<OrderBy><FieldRef Name=""Modified"" Ascending=""FALSE"" /></OrderBy>";
        //        oQuery.RowLimit = 1;
        //        item = sList.GetItems(oQuery)[0];
        //    }
        //    return (T)Activator.CreateInstance(typeof(T), new object[] { item, sWeb });
        //}
        public static T GetSettings<T>(this SPWeb sWeb, string ListName, string filter = null)
        {
            SPList sList = sWeb.Lists.TryGetList(ListName);
            if (filter == null) return (T)Activator.CreateInstance(typeof(T), new object[] { sList.Items, sWeb });
            else
            {
                SPQuery spQuery = new SPQuery();
                spQuery.Query = "<Where>" +
                                        "<Eq>" +
                                            "<FieldRef Name='FilterValue'/>" +
                                            "<Value Type='Text'>" + filter + "</Value>" +
                                        "</Eq>" +
                                     "</Where>";
                SPListItemCollection sListItems = sList.GetItems(spQuery);
                return (T)Activator.CreateInstance(typeof(T), new object[] { sListItems, sWeb });
            }
        }
        public static T GetObject<T>(this SPWeb sWeb, string ListName, int ID)
        {
            SPList sList = sWeb.Lists[ListName];
            SPListItem item = null;
            if (sList != null)
            {
                item = sList.GetItemById(ID);
                if (item == null) return default(T);
            }
            return (T)Activator.CreateInstance(typeof(T), new object[] { item, sWeb });
        }
        public static List<T> GetObject<T>(this SPWeb sWeb, SPList list)
        {
            var returnlist = new List<T>();
            if (list == null) return returnlist;
            SPListItemCollection items = list.Items;
            foreach (var item in items) returnlist.Add((T)Activator.CreateInstance(typeof(T), new object[] { item, sWeb }));
            return returnlist;
        }
        public static List<T> GetObject<T>(this SPWeb sWeb, SPListItemCollection items)
        {
            var returnlist = new List<T>();
            foreach (var item in items) returnlist.Add((T)Activator.CreateInstance(typeof(T), new object[] { item, sWeb }));
            return returnlist;
        }
        public static SPUser GetSPUser(this SPListItem item, string fieldName, SPWeb sWeb)
        {
            SPUser spUser = null;
            SPFieldUserValue userValue = new SPFieldUserValue(sWeb, Convert.ToString(item[fieldName]));
            if (userValue != null)
            {
                spUser = userValue.User;
            }
            return spUser;
        }
        public static string GetSPUserFullName(this SPListItem item, string fieldName, SPWeb sWeb)
        {
            SPUser spUser = null;
            SPFieldUserValue userValue = new SPFieldUserValue(sWeb, Convert.ToString(item[fieldName]));
            if (userValue != null)
            {
                spUser = userValue.User;
                return spUser.Name;
            }
            return "";
        }
        public static string GetSPUserEntityString(this SPListItem item, string fieldName, SPWeb sWeb)
        {
            SPUser spUser = null;
            SPFieldUserValue userValue = new SPFieldUserValue(sWeb, Convert.ToString(item[fieldName]));
            if (userValue != null)
            {
                spUser = userValue.User;
                return spUser.ID + ";#" + spUser.LoginName;
            }
            return "";
        }


        public static SPUser GetSPUser(this SPWeb sWeb, string userLogin)
        {
            SPUser spUser = null;
            SPPrincipalInfo spPrincipalInfo = SPUtility.ResolvePrincipal(sWeb
                                                                            , userLogin
                                                                            , SPPrincipalType.All
                                                                            , SPPrincipalSource.All
                                                                            , null
                                                                            , false);
            if (spPrincipalInfo != null)
            {
                spUser = sWeb.SiteUsers[spPrincipalInfo.LoginName];
            }
            return spUser;
        }
        public static T GetValue<T>(this SPListItem item, string fieldName)
        {
            object value;
            if (fieldName != null) value = item[fieldName]; else value = null;
            if (value == null || value == "") return default(T);
            var t = typeof(T);
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable)))
            {   
                t = Nullable.GetUnderlyingType(t);
            }
            return (T)Convert.ChangeType(value, t);
        }
        public static ListItem[] GetDropdownItems(SPWeb sWeb, string listName, string labelField, string valueField, bool insertBlank = false, string blankString = "")
        {
            var result = new List<ListItem>();
            SPList List = sWeb.Lists.TryGetList(listName);
            if (List != null)
            {
                SPListItemCollection items = List.Items;
                foreach (SPListItem item in items)
                {
                    if (item != null)
                    {
                        var label = Convert.ToString(item[labelField]);
                        var value = Convert.ToString(item[valueField]);
                        if ((!string.IsNullOrEmpty(value)) && (!string.IsNullOrEmpty(label)))
                            result.Add(new ListItem(label, value));
                    }
                }
            }
            if (insertBlank || result.Count == 0)
            {
                result.Insert(0, new ListItem(blankString, "0"));
            }
            return result.ToArray();
        }

        public static ListItem[] GetDropdownChoices(SPWeb sWeb, string listName, string fieldName, bool insertBlank = false, string blankString = "")
        {
            List<ListItem> result = new List<ListItem>();
            SPList sList = sWeb.Lists[listName];
            SPFieldChoice field = (SPFieldChoice)sList.Fields.GetField(fieldName);
            foreach (string str in field.Choices)
            {
                result.Add(new ListItem(str, str));
            }
            if (insertBlank || result.Count == 0)
            {
                result.Insert(0, new ListItem(blankString, ""));
            }
            return result.ToArray();
        }
        public static ListItem[] GetDropdownMultiChoices(SPWeb sWeb, string listName, string fieldName,bool insertBlank = false, string blankString = "")
        {
            List<ListItem> result = new List<ListItem>();
            SPList sList = sWeb.Lists[listName];
            SPFieldMultiChoice field = (SPFieldMultiChoice)sList.Fields.GetField(fieldName);
            foreach (string str in field.Choices)
            {
                result.Add(new ListItem(str, str));
            }
            if (insertBlank || result.Count == 0)
            {
                result.Insert(0, new ListItem(blankString, ""));
            }
            return result.ToArray();
        }
        public static UserProfile GetUserProfile(this SPWeb sWeb, string accountName)
        {
            SPUser u = sWeb.EnsureUser(accountName);
            UserProfile userProfile = null;
            SPSecurity.RunWithElevatedPrivileges
            (delegate
            {
                using (SPSite siteCollection = new SPSite(sWeb.ParentWeb.Url))
                {
                    SPServiceContext serverContext = SPServiceContext.GetContext(siteCollection);
                    UserProfileManager userProfileMangager = new UserProfileManager(serverContext);

                    if (userProfileMangager.UserExists(accountName))
                    {
                        userProfile = userProfileMangager.GetUserProfile(accountName);
                    }
                }
            });
            return userProfile;
        }

        public static string GetEmailsFromSPUserCollection(SPListItem sPListItem, string p)
        {
            Object spUsersObject = sPListItem[p];
            if (spUsersObject != null)
            {
                SPFieldUserValueCollection spUsers = spUsersObject as SPFieldUserValueCollection;
                List<string> emails = new List<string>();
                foreach (SPFieldUserValue item in spUsers) emails.Add(item.User.Email);
                if (emails.Count == 1) return emails[0];
                if (emails.Count > 1) return String.Join(";", emails);
            }
            return "";
        }

        public static void SaveMailToLibrary(SPList docLib, byte[] byteArray, string targetList,string titlePrefix, string status, bool isDelivered)
        {
            if (docLib != null)
            {
                DateTime now = DateTime.Now;

                Hashtable hashtable = new Hashtable();
                hashtable["TTPOMailMessageStatus"] = status;
                hashtable["TTPOMailMessageTargetList"] = targetList;
                hashtable["TTPOIsDelivered"] = isDelivered;

                SPFile file = docLib.RootFolder.Files.Add(titlePrefix + "_" + now.ToString("s").Replace(":", "-") + "-" + now.Millisecond + ".eml", byteArray, hashtable, true);
                //file.Item["Status"] = status;
                //file.Item["TargetList"] = targetList;
                //file.Item["IsDelivered"] = isDelivered;
                file.Update();
            }
        }

        public static string GetIdFromLookup(string value, string defaultValue = "0")
        {
            if (value.IndexOf(";#") >= 0) return value.Split(new Char[]{';','#'})[0];
            else return defaultValue;
        }
    }
}
