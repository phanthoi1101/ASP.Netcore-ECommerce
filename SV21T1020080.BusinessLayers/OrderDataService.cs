﻿using SV21T1020080.DataLayers;
using SV21T1020080.DataLayers.SQLServer;
using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.BusinessLayers
{
    public static class OrderDataService
    {
        private static readonly IOrderDAL orderDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static OrderDataService()
        {
            orderDB = new OrderDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang (trả về 1 list đơn hàng và 1 rown count lấy ra số đếm đươn hàng tìm được)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Order> ListOrders(out int rowCount, int page = 1, int pageSize = 0,int status = 0, 
                                                    DateTime? fromTime = null, DateTime? toTime = null,string searchValue = "")
        {
            rowCount = orderDB.Count(status, fromTime, toTime, searchValue);
            return orderDB.List(page, pageSize, status, fromTime, toTime, searchValue).ToList();
        }
        /// <summary>
        /// Lấy về thông tin của một đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static Order? GetOrder(int orderId)
        {
            return orderDB.Get(orderId);
        }
        public static bool AcceptOrder(int orderID, int employeeID)
        {
            Console.WriteLine("tiến hành accept");

            //if (employeeID == 0)
            //{
            //    Order? data = orderDB.Get(orderID);
            //    if (data == null)
            //        return false;
            //    if (data.Status == Constants.ORDER_INIT)
            //    {
            //        data.Status = Constants.ORDER_ACCRPTED;
            //        data.AcceptTime = DateTime.Now;
            //        return orderDB.Update(data);
            //    }
            //    return false;
            //}
            //else
            //{
                Order? data = orderDB.Get(orderID);
                if (data == null)
                    return false;
                if (data.Status == Constants.ORDER_INIT)
                {
                    data.Status = Constants.ORDER_ACCRPTED;
                    data.AcceptTime = DateTime.Now;
                    data.EmployeeID = employeeID;
                    return orderDB.Update(data);
                }
                return false;
            //}
        }
        /// <summary>
        /// Khởi tạo đơn hàng mới (tạo đơn hàng mới ở trạng thái Init)
        /// Hàm trả về mã của đơn hàng được tạo mới
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="customerID"></param>
        /// <param name="deliveryProvince"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static int InitOrder(int employeeID , int customerID, string deliveryProvince, string deliveryAddress,IEnumerable<OrderDetail> details)
        {
            if (details.Count() == 0)
            {
                return 0;
            }
            Order data = new Order()
            {
                EmployeeID = employeeID,
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
            };
            int orderID = orderDB.Add(data);
            if (orderID > 0)
            {
                foreach (var item in details) { 
                    orderDB.SaveDetail(orderID, item.ProductID,item.Quantity,item.SalePrice);
                }
                return orderID;
            }
            return 0;
        }
        public static List<OrderHistory>? ListOfOrderHistory(int customerID,int status)
        {
            return orderDB.ListOfOrderHistory(customerID,status);
        }
        public static int InitOrderUsers(int customerID, string deliveryProvince, string deliveryAddress, IEnumerable<OrderDetail> details)
        {
            if (details.Count() == 0)
            {
                return 0;
            }
            Order data = new Order()
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
            };
            int orderID = orderDB.Add(data);
            if (orderID > 0)
            {
                foreach (var item in details)
                {
                    orderDB.SaveDetail(orderID, item.ProductID, item.Quantity, item.SalePrice);
                }
                return orderID;
            }
            return 0;
        }
        /// <summary>
        /// huỷ bỏ đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool CancelOrder(int orderID)
        {
            Order? data = orderDB.Get(orderID);
            if (data == null)
                return false;
            if(data.Status != Constants.ORDER_FINISHED)
            {
                data.Status = Constants.ORDER_CANCEL;
                data.FinishedTime = DateTime.Now;
                return orderDB.Update(data);
            }
            return false;

        }
        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool RejectOrder(int orderID)
        {
            Order? data =orderDB.Get(orderID);
            if(data == null) 
                return false;
            if( data.Status == Constants.ORDER_INIT || data.Status== Constants.ORDER_ACCRPTED)
            {
                data.Status = Constants.ORDER_REJECTED;
                data.FinishedTime = DateTime.Now;
                return orderDB.Update(data);
            }
            return false;
        }
        /// <summary>
        /// Duyệt chấp nhận đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>

        /// <summary>
        /// Xác nhận đã chuyển hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool ShipOrder(int orderID,int shipperID)
        {
            Order? data = orderDB.Get(orderID);
            if(data == null)
                return false;
            if(data.Status == Constants.ORDER_ACCRPTED||data.Status== Constants.ORDER_SHIPPING)
            {
                data.Status = Constants.ORDER_SHIPPING;
                data.ShipperID = shipperID;
                data.ShippedTime = DateTime.Now;
                return orderDB.Update(data);
            }
            return false ;
        }
        /// <summary>
        /// Ghi nhận kết thúc quá trình xử lý đơn hàng thành công
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool FinishOrder(int orderID)
        {
            Order? data =orderDB.Get(orderID);
            if(data == null) return false;
            if(data.Status == Constants.ORDER_SHIPPING)
            {
                data.Status = Constants.ORDER_FINISHED;
                data.FinishedTime = DateTime.Now;
                return orderDB.Update(data);
            }
            return false ;  
        }
        /// <summary>
        /// Xoá đơn hàng và toàn bộ chi tiết của đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool DeleteOrder(int orderID)
        {
            var data = orderDB.Get(orderID);
            if(data == null) return false;
            if(data.Status == Constants.ORDER_INIT ||  data.Status== Constants.ORDER_CANCEL || data.Status == Constants.ORDER_REJECTED)
                return orderDB.Delete(orderID);
            return false ;
        }
        /// <summary>
        /// Lấy ra danh sách các mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>

        public static List<OrderDetail> ListOrderDetail(int orderID)
        {
            return orderDB.ListDetails(orderID).ToList();
        }
        /// <summary>
        /// Lấy ra một mặt hàng được bán trong đơn hàng 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static OrderDetail? GetOrderDetail(int orderID,int productID)
        {
            return orderDB.GetDetail(orderID, productID);
        }
        /// <summary>
        /// Lưu thông tin chi tiết của đơn hàng (Thêm mặt hàng được bán trong đơn hàng theo nguyên tăc)
        /// - Nếu mặt hàng chưa có trong chi tiết đơn hàng thì bổ sung mặt hàng vào chi tiết đơn hàng
        /// - Nếu mặt hàng đã có trong chi tiết đơn hàng thì tiến hành cập nhật lại số lượng và giá bán
        /// </summary>
        /// <returns></returns>
        public static bool SaveOrderDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            Order? data = orderDB.Get(orderID);
            if(data == null) return false;
            if(data.Status == Constants.ORDER_INIT||data.Status == Constants.ORDER_ACCRPTED)
            {
                return orderDB.SaveDetail(orderID,productID,quantity,salePrice);
            }
            return false ;
        }
        /// <summary>
        /// Xoá một mặt hàng ra khỏi đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="prodcutID"></param>
        /// <returns></returns>
        public static bool DeleteOrderDetail(int orderID, int prodcutID)
        {
            Order? data = orderDB.Get(orderID);
            if(data == null) return false;
            if(data.Status == Constants.ORDER_INIT || data.Status == Constants.ORDER_ACCRPTED)
                return orderDB.DeleteDetail(orderID,prodcutID);
            return false ;
        }

    }
}
