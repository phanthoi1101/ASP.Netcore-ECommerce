using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DataLayers.SQLServer
{
    public abstract class BaseDAL
    {
        protected string _connectionString="";
        /// <summary>
        /// hàm khởi tạo
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
