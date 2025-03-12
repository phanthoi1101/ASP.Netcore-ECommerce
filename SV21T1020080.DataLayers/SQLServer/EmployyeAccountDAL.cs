using Dapper;
using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DataLayers.SQLServer
{
    public class EmployyeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployyeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection()) 
            {
                var sql = @"select EmployeeID as UserId,
		                            Email as UserName,
		                            FullName as DisPlayName,
		                            Photo,
		                            RoleNames
                            from Employees
                            where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email= username,
                    Password= password,
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql:sql, param: parameters,commandType: System.Data.CommandType.Text);   
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string oldPassword,string newPassword )
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Employees 
                            set Password = @newPassword
                            where Email = @username and Password = @oldPassword";
                var parameters = new
                {
                    username = username,
                    newPassword = newPassword,
                    oldPassword = oldPassword,
                };
                result = connection.Execute(sql:sql,param: parameters,commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }
    }
}
