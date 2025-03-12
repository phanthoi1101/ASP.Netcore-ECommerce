using SV21T1020080.DomainModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SV21T1020080.DataLayers.SQLServer
{
    public class CategoryDAL : BaseDAL, ICommonDAL<Categories>
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Categories item)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                select -1;
                            else 
                                begin
                                    insert into Categories(CategoryName,Description)
                                    values(@CategoryName,@Description)
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    CategoryName = item.CategoryName,
                    Description = item.Description,

                };
                id = connection.ExecuteScalar<int>(sql:sql, param:parameters,commandType:System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue)
        {
            int Count = 0;
            searchValue = $"%{searchValue}%";
            
            using (var connection = OpenConnection() )
            {
                var sql = @"select COUNT(*)
                        from Categories
                        where CategoryName like @SearchValue or Description LIKE @SearchValue";
                var parameters = new
                {
                    searchValue = searchValue,


                };
                Count = connection.ExecuteScalar<int>( sql:sql, param:parameters, commandType: System.Data.CommandType.Text);
            }

            return Count;
        }

        public int Create(Categories data, string pass)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete Categories
                            where CategoryID = @CategoryID";

                var parameters = new
                {
                    CategoryID = id,
                };
                result = connection.Execute(sql:sql , param:parameters , commandType:System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }

        public Categories? Get(int id)
        {
            Categories? categories = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from Categories 
                            where CategoryID= @CategoryID";
                var parameters = new
                {
                    CategoryID = id,
                };
                
                categories = connection.QueryFirst<Categories>(sql:sql,param:parameters , commandType: System.Data.CommandType.Text);
                connection.Close ();
            }
            return categories;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if(exists(select * from Products where CategoryID = @CategoryID))
	                            select 1 
                            else
	                            select 0";
                var parameters = new
                {
                    CategoryID = id,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Categories> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Categories> list = new List<Categories>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
		                            select * , ROW_NUMBER() over (order by CategoryName ) as rowNumber
		                            from Categories 
		                            where (CategoryName like @searchValue) or (Description like @searchValue)) as t
                            where (@pageSize = 0) 
	                            or (rowNumber between (@page-1)*@pageSize+1 and @page*@pageSize)
                              order by rowNumber";
                var parameters = new
                {
                    searchValue = searchValue,
                    page = page,
                    pageSize = pageSize,
                };
                list = connection.Query<Categories>(sql:sql , param: parameters , commandType: System.Data.CommandType.Text).ToList();
            }
            return list;
        }

        public bool Update(Categories data)
        {
            var result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Categories where CategoryID<>@CategoryID and CategoryName = @CategoryName)
                            begin
                                update Categories
                                set CategoryName= @CategoryName,
	                                Description = @Description
                                where  CategoryID = @CategoryID
                            end;";
                var parameters = new
                {
                    CategoryID = data.CategoryID,
                    CategoryName = data.CategoryName,
                    Description = data.Description,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close ();
            }
            return result;
        }
    }
}
