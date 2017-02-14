using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Square9.SecurityDisplay.Logic
{
    public class UsersLogic
    {
        public List<String> GetUsersOfGroup(string DomainOrServerName, string GroupName, bool domain = true)
        {
            List<String> Users = new List<String>();

            var context = new PrincipalContext(ContextType.Machine, DomainOrServerName);

            if (domain)
            {
                context = new PrincipalContext(ContextType.Domain, DomainOrServerName);
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

                    foreach  (UserPrincipal member in group.GetMembers())
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
    }
}