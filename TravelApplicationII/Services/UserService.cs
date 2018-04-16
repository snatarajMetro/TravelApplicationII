using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using TravelApplication.DAL.Repositories;
using TravelApplication.Models;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;

namespace TravelApplication.Services
{
    public class UserService : IUserService
    {
        IUserRepository userRepo = new UserRepository();
        private static string LDAP_SERVER = ConfigurationManager.AppSettings["ldapServer"];
        private static string LDAP_SEARCH_PATH = ConfigurationManager.AppSettings["ldapServerPath"];
        public UserRole ValidateAndGetRoles(UserModel userModel)
        {
            var response = userRepo.ValidateAndGetRoles(userModel);
            return response;
        }

        public bool IsValidADUser(string username, string password)
        {      
            bool userValidationFlag = true;
            var credential = new NetworkCredential(username, password);
            var serverId = new LdapDirectoryIdentifier(LDAP_SERVER);
            var conn = new LdapConnection(serverId, credential);

            try
            {
                conn.Bind();
            }
            catch (Exception)
            {
                userValidationFlag = false;
            }
            finally
            {
                if (conn != null) conn.Dispose();
            }

            return userValidationFlag;
        }

    }
}