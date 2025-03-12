using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using SV21T1020080.Web.AppCodes;
using SV21T1020080.Web.Models;
using System.Globalization;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.SALE}")]
    public class OrderController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        private const string SHOPPING_CART = "ShoppingCart";

        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    Status = 0,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    ToTime = null,
                        FromTime = null,
                        TimeRange = $"{DateTime.Today.AddYears(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
            OrderSearchResult model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }

        public IActionResult SearchProduct(OrderSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");

            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetail(id);
            var model = new OrderDetailModel
            {
                Order = order,
                Details = details
            };
            return View(model);
        }
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id, productId);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult UpdateDetail(int id ,OrderDetail data)
        {
            if (data.Quantity <= 0)
                ModelState.AddModelError(nameof(data.Quantity), "Số lượng không hợp lệ");
            if (data.SalePrice <= 0)
                ModelState.AddModelError(nameof(data.SalePrice), "Gia bán không không hợp lệ");
            if (!ModelState.IsValid)
            {
                return View("EditDetail", data);
            }
            bool result = OrderDataService.SaveOrderDetail(id, data.ProductID, data.Quantity, data.SalePrice);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult DeleteDetail(int id , int productId)
        {
                OrderDataService.DeleteOrderDetail(id, productId);
                return RedirectToAction("Details", new {id=id});

        }
        public IActionResult Delete(int id) { 
            OrderDataService.DeleteOrder(id);
            return RedirectToAction("Index");
        }
        public IActionResult Cancel(int id)
        {
            OrderDataService.CancelOrder(id);
            return RedirectToAction("Details",new {id=id});
        }
        public IActionResult Reject(int id)
        {
            OrderDataService.RejectOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Shipping(int id, int shipperID)
        {
            var data = OrderDataService.GetOrder(id);
            if (data == null)
                return RedirectToAction("Index");

            if (Request.Method == "POST")
            {
                if (shipperID == 0)
                {
                    return Json("Vui lòng chọn người giao hàng");
                }

                OrderDataService.ShipOrder(id, shipperID);
                return RedirectToAction("Details", new { id });
            }
            return View(data);
        }
        public IActionResult Accept(int id)
        {
            WebUserData? data = User.GetUserData();
            int employeeID = int.Parse(data.UserId);
            Console.WriteLine("Đơn hàng đã được duyệt");
            Console.WriteLine(employeeID);
            OrderDataService.AcceptOrder(id,employeeID);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
            }

            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            }

            WebUserData? data = User.GetUserData();
            int employeeID = int.Parse(data.UserId);

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
        public IActionResult Finish(int id)
        {
            OrderDataService.FinishOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }



        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m=>m.ProductID == id);
            if(index >= 0) 
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART,shoppingCart);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }

        

       
    }
}
