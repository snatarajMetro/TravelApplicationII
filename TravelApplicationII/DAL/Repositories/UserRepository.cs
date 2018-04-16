using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using TravelApplication.DAL.DBProvider;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private DbConnection dbConn;
        public UserRole ValidateAndGetRoles(UserModel userModel)
        {
            var userResult = GetUser(userModel);
            if (userResult.result)
            {
                var response = GetUserRole(userResult.userId, userResult.userName);
                if (response.Roles.Any())
                {
                    response.BadgeNumber = userResult.BadgeNumber;
                    return response;
                }
                else
                {
                    throw new Exception("User does not have any roles assigned");
                }
            }
            else
            {
                throw new Exception("User does not exists");
            }
           
        }

        public UserResponse GetUser(UserModel request)
        {
            UserResponse response = null;
            using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
            {
                string query = string.Format("Select * from Users where LoginId = '{0}' ", request.UserName);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {

                        response = new UserResponse() { result = true, userId = Convert.ToInt32(dataReader["Id"]), userName = dataReader["LoginId"].ToString(), BadgeNumber = Convert.ToInt32(dataReader["BAdgeNumber"]) };
                    }
                }
                else
                {
                    response = new UserResponse() { result = false };
                }
                dataReader.Close();
                command.Dispose();
                dbConn.Close();
                dbConn.Dispose();
            }

            return response;
        }

        public UserRole GetUserRole(int userId, string userName)
        {
            UserRole ur = null;
            using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
            {
                IList<Role> items = new List<Role>();
                string query = string.Format("SELECT Id, description FROM ROLES  r INNER JOIN USERROLEMAP  u on r.ID = u.ROLEID where u.USERID = {0}", userId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        Role r = new Role() { Id = Convert.ToInt32(dataReader["Id"]), Name = dataReader["Description"].ToString() };
                        items.Add(r);
                    }
                    ur = new UserRole() { UserId = userId,  UserName= userName, Roles = items };
                }
                dataReader.Close();
                command.Dispose();
                dbConn.Close();
                dbConn.Dispose();
                return ur;
            }

        }
    }
}