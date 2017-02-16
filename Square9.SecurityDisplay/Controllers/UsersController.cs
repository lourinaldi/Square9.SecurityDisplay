using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Square9.SecurityDisplay.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        //(api/users/groupusers?DomainOrServerName={computer name/domain}&GroupName={group name}&Domain=false)
        [HttpGet]
        [ActionName("groupusers")]
        public List<string> Get(string DomainOrServerName, string GroupName, bool Domain = true)
        {
            List<string> Users = new List<string>();

            try
            {
                var logic = new Square9.SecurityDisplay.Logic.UsersLogic();
                Users = logic.GetUsersOfGroup(DomainOrServerName, GroupName, Domain);
            }
            catch (Exception)
            {

                throw;
            }

            return Users;
        }

        //(api/users/tree?DomainOrServerName={computer name/domain}&UserName={username}&Password={password})
        [HttpGet]
        [ActionName("tree")]
        public List<Models.SecurityNode> GetUserTree(string DomainOrServerName, string UserName, string Password)
        {
            List<Models.SecurityNode> Users = new List<Models.SecurityNode>();

            try
            {
                var logic = new Square9.SecurityDisplay.Logic.UsersLogic();
                Users = logic.GetSecuredUsersTree(DomainOrServerName, UserName, Password);
            }
            catch (Exception)
            {

                throw;
            }

            return Users;
        }

        //(api/users/tree?DomainOrServerName={computer name/domain}&UserName={username}&Password={password})
        [HttpGet]
        [ActionName("secured")]
        public List<Models.SecuredGroup> GetSecuredUserList(string DomainOrServerName, string UserName, string Password, string Secured = "secured")
        {
            List<Models.SecuredGroup> Users = new List<Models.SecuredGroup>();

            try
            {
                var logic = new Square9.SecurityDisplay.Logic.UsersLogic();
                Users = logic.GetSecuredUsers(DomainOrServerName, UserName, Password);
            }
            catch (Exception)
            {

                throw;
            }

            return Users;
        }

        //(api/users/tree?DomainOrServerName={computer name/domain}&UserName={username}&Password={password})
        [HttpGet]
        [ActionName("permissions")]
        public List<Models.Permissions> GetAllDatabasePermissions(string DomainOrServerName, string UserName, string Password, int DatabaseID, bool domain = true)

        {
            List<Models.Permissions> permissions = new List<Models.Permissions>();

            try
            {
                var logic = new Square9.SecurityDisplay.Logic.UsersLogic();
                permissions = logic.GetAllDatabasePermissions(DomainOrServerName, UserName, Password, DatabaseID, domain);
            }
            catch (Exception)
            {

                throw;
            }

            return permissions;
        }
    }
}
