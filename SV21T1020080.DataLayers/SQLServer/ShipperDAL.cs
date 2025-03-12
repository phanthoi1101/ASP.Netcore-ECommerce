using Dapper;
using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DataLayers.SQLServer
{
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper item)
        {
            int id = 0;
            using (var connetion = OpenConnection())
            {
                var sql = @"if exists(select * from Shippers where Phone = @Phone)
                                select -1;
                            else 
                                begin
                                    insert into Shippers(ShipperName , Phone)
                                    values (@ShipperName , @Phone)
                                    select SCOPE_IDENTITY();
                                end";
                var parameters =  new
                {
                    ShipperName = item.ShipperName,
                    Phone = item.Phone,
                };
                id = connetion.ExecuteScalar<int>(sql:sql, param:parameters,commandType:System.Data.CommandType.Text);
                connetion.Close();
            }
            return id;
        }

        public int Count(string searchValue)
        {
            int Count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Shippers
                            where (ShipperName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                Count = connection.ExecuteScalar<int>(sql : sql, param:parameters , commandType: System.Data.CommandType.Text);
            }

                return Count;
        }

        public int Create(Shipper data, string pass)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connetion = OpenConnection())
            {
                var sql = @"delete Shippers
                            where ShipperID= @ShipperID
                            ";
                var parameters = new
                {
                    ShipperID = id,
                };
                result = connetion.Execute(sql:sql , param: parameters , commandType:System.Data.CommandType.Text)>0;
                connetion.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? shipper = null;
            using ( var connetion = OpenConnection())
            {
                var sql = @"select * 
                            from Shippers
                            where ShipperID = @ShipperID
                            ";
                var parameters = new
                {
                    ShipperID= id,
                };
                shipper = connetion.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connetion.Close();
            }
            return shipper;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using ( var connetion = OpenConnection())
            {
                var sql = @"if(exists(select * from Orders where ShipperID = @ShipperID))
	                            select 1
                            else 
	                            select 0";
                var parameters = new
                {
                    ShipperID = id,
                };
                result = connetion.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connetion.Close();
            }
            return result;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> list = new List<Shipper>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
		                            select * , ROW_NUMBER() over (order by ShipperName ) as rowNumber
		                            from Shippers 
		                            where (ShipperName like @searchValue)) as t
                            where (@pageSize = 0) 
	                            or (rowNumber between (@page-1)*@pageSize+1 and @page*@pageSize)
                              order by rowNumber";
                var parameters = new
                {
                    searchValue = searchValue,
                    page = page,
                    pageSize = pageSize,
                };

                list = connection.Query<Shipper>(sql:sql , param:parameters , commandType: System.Data.CommandType.Text).ToList();
            }


            return list;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connetion = OpenConnection())
            {
                var sql = @"if not exists(select * from Shippers where ShipperID<>@ShipperID and Phone = @Phone)
                            begin
                            update Shippers
                            set ShipperName = @ShipperName,
	                            Phone = @Phone
                            where ShipperID = @ShipperID
                            end";
                var parameters = new
                {
                    ShipperID = data.ShipperID,
                    ShipperName = data.ShipperName,
                    Phone = data.Phone,
                };
                result = connetion.Execute(sql:sql , param:parameters , commandType: System.Data.CommandType.Text)>0;
                connetion.Close();
            }
            return result;
        }
    }
}
