using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using SV21T1020080.Shop.AppCodes;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020080.Shop.Controllers
{
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        [Authorize]
        public IActionResult Index()
        {
            List<CartItem> shoppingCart = GetShoppingCart();
            decimal total=0;
            foreach(var item in shoppingCart)
            {
                total = total + item.TotalPrice;
            }
            ViewBag.totalPrice = total;
            return View(shoppingCart);
        }
        public IActionResult LichSuDonHang(int id=1)
        {
            ViewBag.ActiveStatus = id;
            Console.WriteLine(id);
            int customerId = int.Parse(User.GetUserData().UserId);
            List<OrderHistory>? data = OrderDataService.ListOfOrderHistory(customerId, id);
            return View(data);
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
        [Authorize]
        [HttpPost]
        public IActionResult AddToCart(CartItem item)
        {
            if (item.Quantity <= 0)
                return Json("Số lượng không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        [HttpPost]
        public IActionResult UpdateCart(int ProductID, int Quantity)
        {
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID ==ProductID);
            if (existsProduct != null)
            {
                existsProduct.Quantity = Quantity;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            decimal total = 0;
            foreach (var item in shoppingCart)
            {
                total = total + item.TotalPrice;
            }
            ViewBag.totalPrice = total;
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            decimal total = 0;
            foreach (var item in shoppingCart)
            {
                total = total + item.TotalPrice;
            }
            ViewBag.totalPrice = total;
            return View("Index", shoppingCart);
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return View("Index", shoppingCart);
        }
        public IActionResult DiaChi()
        {
            var userdata = User.GetUserData();
            int userID = 0;
            if(userdata != null)
            {
                 userID = int.Parse(userdata.UserId);
            }
            var customer = CommonDataService.GetCustomer(userID);
            return View("DiaChi", customer);
        }
        [HttpPost]
        public IActionResult Init(int CustomerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            Console.WriteLine(CustomerID);
            var shoppingCart = GetShoppingCart();
            WebUserData? data = User.GetUserData();
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
            int orderID = OrderDataService.InitOrderUsers( CustomerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return RedirectToAction("LichSuDonHang",1);
        }
    }
    }
