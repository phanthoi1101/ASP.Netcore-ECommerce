using Azure;
using SV21T1020080.DataLayers;
using SV21T1020080.DomainModels;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Categories> categoriesDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        static CommonDataService() 
        {
            string connectionString = Configuration.ConnectionString;
            provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);
            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            categoriesDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
            shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
        }
        /// <summary>
        /// lấy ra danh sách các tỉnh thành của đối tượng tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvince()
        {
            return provinceDB.List();
        }
        /// <summary>
        /// tìm kiếm và lấy ra danh sách dưới dạng phân trang 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> LitOfCustomers(out int rowCount, int page, int pageSize, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page,pageSize,searchValue);  
        }
        /// <summary>
        /// Lấy về một khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }

        /// <summary>
        /// thêm một khách hàng vào cơ sở dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        public static int CreateCustomer(Customer data,string pass)
        {
            return customerDB.Create(data, pass);
        }

        /// <summary>
        /// Cập nhật một khách hàng vào cơ sở dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }

        /// <summary>
        /// Xoá danh sách 1 khách hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }


        /// <summary>
        /// trả về khách hàng đã đặt hàng hay chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }


        /// <summary>
        /// Hàm trả về 1 list danh sách các nhà cung cấp
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSupliers(out int rowCount , int page , int pageSize , string searchValue="")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page,pageSize,searchValue);
        }

        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);
        }

        public static bool UpdateSupplier(Supplier supplier)
        {
            return supplierDB.Update(supplier);
        }

        public static Supplier GetSuplier(int id)
        {
            return supplierDB.Get(id);
        }

        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }

        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }
        
        //CATEGORY
        public static List<Categories> ListOfCategories(out int rowCount , int page , int pageSize , string searchValue)
        {
            rowCount = categoriesDB.Count(searchValue);
            return categoriesDB.List(page,pageSize, searchValue);
        }
        public static int AddCategory(Categories categories)
        {
            return categoriesDB.Add(categories);
        }

        public static bool DeleteCategory(int id)
        {
            return categoriesDB.Delete(id);
        }

        public static bool InUsedCategory(int id)
        {
            return categoriesDB.InUsed(id);
        }

        public static bool UpdateCategory(Categories categories)
        {
            return categoriesDB.Update(categories);
        }
        public static Categories GetCategory(int id)
        {
            return categoriesDB.Get(id);
        }

        public static List<Shipper> ListOfShippers(out int rowCount , int page , int pageSize , string searchValue)
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page,pageSize,searchValue) ;
        }
        public static int AddShipper(Shipper shipper)
        {
            return shipperDB.Add(shipper);
        }
        public static Shipper GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }

        /// <summary>
        /// Bổ sung 1 khách hàng, hàm trả về id của khách hàng được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của 1 khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }

        /// <summary>
        /// Xóa 1 khách hàng có mã là id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }

        /// <summary>
        /// Kiểm tra xem 1 khách hàng hiện đang có đơn hàng liên quan hay là không
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }


    }
}
