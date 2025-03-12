using SV21T1020080.DataLayers;
using SV21T1020080.DataLayers.SQLServer;
using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            employeeAccountDB = new DataLayers.SQLServer.EmployyeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(UserTypes userType,string username , string password)
        {
            if(userType == UserTypes.Employee)
                return employeeAccountDB.Authorize (username, password);
            else 
                return customerAccountDB.Authorize (username, password);
        }
        public static bool ChancePassword(string username , string oldPassword , string newPassword)
        {
            return employeeAccountDB.ChangePassword (username , oldPassword , newPassword);
        }
        public static bool ChancePasswordCustomer(string username, string oldPassword, string newPassword)
        {
            return customerAccountDB.ChangePassword(username, oldPassword, newPassword);
        }
    }
    public enum UserTypes
    {
        Employee,
        Customer
    }
}
