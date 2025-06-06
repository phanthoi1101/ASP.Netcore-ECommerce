﻿using Dapper;
using System;
using SV21T1020080.DomainModels;
using System.Data;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SV21T1020080.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu luên quan đến khách hàng (bảng Customers)
    /// </summary>
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        /// <summary>
        /// khởi tạo
        /// </summary>
        /// <param name="connectionString"></param>
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// thêm 1 khách hàng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Add(Customer item)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1;
                            else 
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                                    values (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    CustomerName = item.CustomerName ?? "",
                    ContactName = item.ContactName ??"",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    IsLocked = item.IsLocked,
                };
                id = connection.ExecuteScalar<int>(sql:sql, param:parameters , commandType: CommandType.Text);
                //thực thi câu lệnh 
                connection.Close();
            }

            return id;
        }


        /// <summary>
        /// đếm số lượng khách hàng tìm kiếm được thông qua keyword
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Customers
                            where (CustomerName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);

            }
            return count;
        }

        public int Create(Customer item, string pass)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1;
                            else 
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email,Password, IsLocked)
                                    values (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email,@Password, @IsLocked);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    CustomerName = item.CustomerName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Password = pass,
                    IsLocked = item.IsLocked,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                //thực thi câu lệnh 
                connection.Close();
            }

            return id;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = id,
                };
               result= connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close ();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? item = null;
            using (var connection = OpenConnection())
            {
                var sql = @"Select * from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID= id,

                };
                item = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return item;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders  where CustomerID = @CustomerID)
	                            select 1
                            else 
	                            select 0";
                var parameters = new
                {
                    CustomerID = id,
                };

                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close() ;

            }


            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
                                  select * , ROW_NUMBER() over (order by CustomerName ) as rowNumber
		                            from Customers 
		                            where (CustomerName like @searchValue) or (ContactName like @searchValue)) as t
                            where (@pageSize = 0) 
	                            or (rowNumber between (@page-1)*@pageSize+1 and @page*@pageSize)
                              order by rowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Customer>(sql:sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false ;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerID<>@CustomerID and Email = @Email)
                            begin
                                update Customers
                                set CustomerName = @CustomerName,	
	                                ContactName = @ContactName,
	                                Province = @Province,
	                                Address = @Address,
	                                Phone = @Phone,
	                                Email = @Email,
	                                IsLocked = @IsLocked
                                  where CustomerID = @CustomerID
                              end";
                var parameters = new
                {
                    CustomerID= data.CustomerID,
                    CustomerName= data.CustomerName,
                    ContactName = data.ContactName,
                    Province = data.Province,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    IsLocked = data.IsLocked,
                };
               
                result = connection.Execute(sql:sql, param: parameters, commandType: CommandType.Text) > 0 ;
                connection.Close();
            }
            return result;
        }
    }
}
