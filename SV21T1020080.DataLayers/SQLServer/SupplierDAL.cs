using Dapper;
using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
    {
        /// <summary>
        /// HÀM KHỞI TẠO
        /// </summary>
        /// <param name="connectionString"></param>
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// THÊM MỚI MỘT SUPPLIER
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Add(Supplier item)
        {
            int result = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Suppliers where Email = @Email)
                                select -1;
                            else 
                                begin
                                    insert into Suppliers(SupplierName,ContactName,Provice,Address,Phone,Email)
                                    values(@SupplierName,@ContactName,@Province,@Address,@Phone,@Email)
                                    select SCOPE_IDENTITY()
                                end";
                var parameters = new
                {
                    SupplierName = item.SupplierName,
                    ContactName = item.ContactName,
                    Province = item.Provice,
                    Address = item.Address,
                    Phone = item.Phone,
                    Email = item.Email,

                };
                result = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// ĐẾM SỐ DÒNG DỮ LIỆU CÓ TRONG DANH SÁCH THÔNG QUA 1 KEYWORD
        /// </summary>
        /// <param name="searchValue">THÔNG TIN MUỐN DANH SÁCH CẦN ĐẾM</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Count(string searchValue)
        {
            int Count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Suppliers
                            where (SupplierName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                Count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return Count;

        }

        public int Create(Supplier data, string pass)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete Suppliers
                            where SupplierID=@SupplierID";
                var parameters = new
                {
                    SupplierID = id,
                };
                result = connection.Execute(sql:sql , param:parameters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result ;
        }

        public Supplier? Get(int id)
        {
            Supplier? item = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from Suppliers
                            where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id,
                };
                item = connection.QueryFirst<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return item;

        }

        public bool InUsed(int id)
        {
            bool result = false ;
            using (var connection = OpenConnection())
            {
                var sql = @"if(exists(select * from Products where SupplierID = @SupplierID))
                                select 1
                            else
                                select 0";
                var parameters = new
                {
                    SupplierID = id,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result ;
        }

        /// <summary>
        /// TRẢ VỀ MỘT DANH SÁCH TƯƠNG ỨNG (1: SỐ DÒNG MUỐN LẤY, SỐ TRANG MUỐN LÂY, THEO KEYWORD NHẬP VÀO)
        /// </summary>
        /// <param name="page">ĐƯA VÀO SỐ TRANG</param>
        /// <param name="pageSize">SỐ DÒNG TRONG 1 TRANG</param>
        /// <param name="searchValue">KEYWORD NHẬP VÀO ĐỂ TÌM KIẾM</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> list = new List<Supplier>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from (select * , ROW_NUMBER() over (order by SupplierName) as rowNumber
			                            from Suppliers
			                            where (SupplierName like @searchValue) or (ContactName like @searchValue)) as t
                            where (@pageSize=0) or rowNumber between (@page-1)*@pageSize + 1 and @page*@pageSize";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };

                list = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();

            }
            return list;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers where SupplierID<>@SupplierID and Email = @Email)
                            begin
                            update Suppliers
	                        set SupplierName = @SupplierName,
		                        Email = @Email,
		                        Phone = @Phone,
		                        Address = @Address,
		                        Provice = @Province,
		                        ContactName = @ContactName
		                        where SupplierID = @SupplierID
                            end";
                var parameters = new
                {
                    SupplierName = data.SupplierName,
                    Email = data.Email,
                    Phone = data.Phone,
                    Address = data.Address,
                    Province = data.Provice,
                    ContactName = data.ContactName,
                    SupplierID = data.SupplierID,
                };
                result = connection.Execute(sql:sql, param: parameters, commandType:System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }
    }
}
