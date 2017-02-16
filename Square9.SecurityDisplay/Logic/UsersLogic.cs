using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Hosting;

namespace Square9.SecurityDisplay.Logic
{
    public class UsersLogic
    {
        public string Domain;
        public string UserName;
        public string Password;

        public List<String> GetUsersOfGroup(string DomainOrServerName, string GroupName, bool domain = true)
        {
            List<String> Users = new List<String>();
                var context = new PrincipalContext(ContextType.Machine, DomainOrServerName);

                if (domain)
                {
                    context = new PrincipalContext(ContextType.Domain, DomainOrServerName, "ntrue", "1M5utwG1");
                }

                try
                {
                    using (var searcher = new PrincipalSearcher())
                    {
                        var sp = new GroupPrincipal(context, GroupName);
                        searcher.QueryFilter = sp;
                        var group = searcher.FindOne() as GroupPrincipal;

                        if (group == null)
                            throw new Exception("Invalid Group Name: " + GroupName);

                        foreach (UserPrincipal member in group.GetMembers())
                        {
                            if (member == null || string.IsNullOrEmpty(member.Name))
                                continue;

                            Users.Add(member.SamAccountName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to return list of Users from Group:  " + ex.Message);
                }

                return Users;
        }

        public List<Models.SecuredGroup> GetSecuredUsers(string domain, string UserName, string Password)
        {
            List<Models.SecuredGroup> SecuredUsers = new List<Models.SecuredGroup>();
            try
            {
                var api = new Requests.ConnectorApi(ConfigurationManager.AppSettings["Square9Api"], domain + @"\" + UserName, Password);
                var license = api.Requests.Licenses.GetLicense();
                SecuredUsers = api.Requests.UserRequests.GetUsersAndGroups(license.Token);
                api.Requests.Licenses.ReleaseLicense(license.Token);
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to return list of Secured Users:  " + ex.Message);
            }
            
            return SecuredUsers;
        }

        public List<String> GetGroupUsers(String DomainOrServerName, String GroupName, bool domain)
        {
            List<String> Users = new List<String>();
            try
            {
                var api = new Requests.ConnectorApi(ConfigurationManager.AppSettings["Square9Api"], domain + @"\" + UserName, Password);
                var license = api.Requests.Licenses.GetLicense();
                Users = api.Requests.UserRequests.GetUserGroups(DomainOrServerName, GroupName, domain);
                api.Requests.Licenses.ReleaseLicense(license.Token);
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to return list of Secured Users:  " + ex.Message);
            }

            return Users;
        }

        public List<Models.SecurityNode> GetSecuredUsersTree(string domain, string UserName, string Password)
        {
            List<Models.SecurityNode> SecuredUsers = new List<Models.SecurityNode>();
            try
            {
                var api = new Requests.ConnectorApi(ConfigurationManager.AppSettings["Square9Api"], domain + @"\" + UserName, Password);
                var license = api.Requests.Licenses.GetLicense();
                SecuredUsers = api.Requests.UserRequests.GetUsersAndGroupsTree(license.Token);
                api.Requests.Licenses.ReleaseLicense(license.Token);
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to return list of Secured Users:  " + ex.Message);
            }

            return SecuredUsers;
        }

        public int GetUserArchiveSecurity(String domain, Int32 DatabaseId, Int32 ArchiveId, String User, String UserName, String Password)
        {
            var security = 0;
            try
            {
                var api = new Requests.ConnectorApi(ConfigurationManager.AppSettings["Square9Api"], domain + @"\" + UserName, Password);
                var license = api.Requests.Licenses.GetLicense();
                security = api.Requests.UserRequests.GetUserArchiveSecurity(DatabaseId, ArchiveId, User, license.Token);
                api.Requests.Licenses.ReleaseLicense(license.Token);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to return list of Secured Users:  " + ex.Message);
            }

            return security;
        }

        public List<Models.Permissions> GetAllDatabasePermissions(String DomainOrServerName, String UserName, String Password, int DatabaseID, bool domain)
        {
            try
            {
                List<Models.Permissions> permissions = new List<Models.Permissions>();

                var logic = new UsersLogic();
                logic.Domain = DomainOrServerName;
                logic.UserName = UserName;
                logic.Password = Password;

                List<Models.SecuredGroup> SecuredUsers = logic.GetSecuredUsers(Domain, UserName, Password);

                foreach (var entry in SecuredUsers)
                {
                    var db = entry.SecuredDBs.FirstOrDefault(d => d.Id == DatabaseID);

                    if (db != null)
                    {

                        List<Models.SecurityNode> tree = logic.GetSecuredUsersTree(Domain, UserName, Password);
                        var databaseNode = tree.Where(t => t.DbId == DatabaseID);

                        foreach (var DBNode in databaseNode)
                        {
                            var archivesNode = DBNode.Children.Where(x => x.Type == "archive");
                            foreach (var archive in archivesNode)
                            {
                                //Is a Group
                                if (entry.Type == 0)
                                {

                                    //var groupUsers = logic.GetUsersOfGroup(DomainOrServerName, entry.Name, domain);
                                    var groupUsers = logic.GetGroupUsers(DomainOrServerName, entry.Name, domain);

                                    var permission = logic.setPermissions(DBNode.DbId, DBNode.Label, archive.Id, archive.Label, entry.Name);
                                    if(permission != null){
                                        permissions.Add(permission);
                                    }
                                    

                                    foreach (var user in groupUsers)
                                    {
                                        if (permission != null)
                                        {
                                            permission.User = user;
                                            permissions.Add(permission);
                                        }

                                    }
                                }
                                //Is a User 
                                else
                                {
                                    var permission = logic.setPermissions(DBNode.DbId, DBNode.Label, archive.Id, archive.Label, "", entry.Name);
                                    if (permission != null)
                                    {
                                        permissions.Add(permission);
                                    }
                                }
                            }
                        }
                    }
                }
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get permissions: " + ex.Message);
            }
            
        }

        public Models.Permissions setPermissions(int DatabaseID, String DatabaseName, int ArchiveID, String ArchiveName, String GroupName = "", String User = "")
        {
            Models.Permissions permission = new Models.Permissions();

            permission.DatabaseID = DatabaseID;
            permission.DatabaseName = DatabaseName;
            permission.Group = GroupName;
            permission.User = User;
            permission.ArchiveID = ArchiveID;
            permission.ArchiveName = ArchiveName;
            var logic = new UsersLogic();
            var securityNumber = 0;
            if (!String.IsNullOrEmpty(GroupName))
            {
                securityNumber = logic.GetUserArchiveSecurity(Domain, permission.DatabaseID, permission.ArchiveID, GroupName, UserName, Password);
            }
            else
            {
                securityNumber = logic.GetUserArchiveSecurity(Domain, permission.DatabaseID, permission.ArchiveID, User, UserName, Password);
            }
                

            if(securityNumber == 0)
            {
                return null;
            }

            var userPermission = (Models.Enumerations.Property)securityNumber;

            #region Folder Level Security
            if (userPermission.HasFlag(Models.Enumerations.Property.ViewDocuments))
            {
                permission.FolderLevel.View = true;
            }

            if(userPermission.HasFlag(Models.Enumerations.Property.AddNewDocuments))
            {
                permission.FolderLevel.Add = true;
            }

            if(userPermission.HasFlag(Models.Enumerations.Property.DeleteDocuments))
            {
                permission.FolderLevel.Delete = true;
            }

            if(userPermission.HasFlag(Models.Enumerations.Property.MoveDocuments))
            {
                permission.FolderLevel.Move = true;
            }

            if(userPermission.HasFlag(Models.Enumerations.Property.ViewDocumentRevisions))
            {
                permission.FolderLevel.ViewRevisions = true;
            }

            if(userPermission.HasFlag(Models.Enumerations.Property.ViewDocumentHistory))
            {
                permission.FolderLevel.ViewHistory = true;
            }

            if(userPermission.HasFlag(Models.Enumerations.Property.DeleteBatches))
            {
                permission.FolderLevel.DeleteErroredBatches = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.APIFullAccess))
            {
                permission.FolderLevel.APIFullAccess = true;
            }
            #endregion
            #region Document Level Security
            if (userPermission.HasFlag(Models.Enumerations.Property.ModifyDocuments))
            {
                permission.DocumentLevel.ModifyDocument = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.ModifyDocumentPages))
            {
                permission.DocumentLevel.ModifyPages = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.ModifyData))
            {
                permission.DocumentLevel.ModifyData = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.ModifyAnnotations))
            {
                permission.DocumentLevel.ModifyAnnotations = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.PublishDocumentRevisions))
            {
                permission.DocumentLevel.PublishRevisions = true;
            }
            #endregion
            #region Export Level Security
            if (userPermission.HasFlag(Models.Enumerations.Property.PrintDocuments))
            {
                permission.ExportLevelSecurity.Print = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.EmailDocuments))
            {
                permission.ExportLevelSecurity.Email= true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.ExportDocuments))
            {
                permission.ExportLevelSecurity.ExportDocument = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.ExportData))
            {
                permission.ExportLevelSecurity.ExportData = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.ViewInAcrobat))
            {
                permission.ExportLevelSecurity.ViewInAcrobat = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.LaunchDocument))
            {
                permission.ExportLevelSecurity.Launch = true;
            }

            if (userPermission.HasFlag(Models.Enumerations.Property.LaunchNewVersion))
            {
                permission.ExportLevelSecurity.LaunchCopy = true;
            }
            #endregion

            return permission;
        }


    }
}