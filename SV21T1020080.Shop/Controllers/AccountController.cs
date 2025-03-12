using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System;

namespace SV21T1020080.Shop.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(int id=0)
        {
            ViewBag.Action = id;
            return View();
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password,int id)
        {
            ViewBag.Action = id;
            ViewBag.UserName = username;
            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }
            //Kiểm tra xem user có đúng hay không
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đang nhập thất bại");
                return View();
            }

            //Đăng nhập thành công
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList(),
            };
            //2. Ghi nhận trạng thái nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());
            //Quay về trang chủ
            Console.WriteLine("text lần 2");
            Console.WriteLine(id);
            if (id != 0)
            {
               return RedirectToAction("ViewProductDetail", "Product", new { id = id });
            }
            return RedirectToAction("Index", "Product");
        }
        public IActionResult Profile(int id)
        {
            var customer = CommonDataService.GetCustomer(id);
            return View(customer);
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Product");
        }
        [HttpPost]
        public IActionResult UpdateProfile(Customer data)
        {
            // Kiểm tra dữ liệu đầu vào và thêm lỗi vào ModelState
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên liên hệ không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");

            // Nếu ModelState không hợp lệ, render lại view kèm theo lỗi
            if (!ModelState.IsValid)
            {
                return View("Profile", data);
            }

            // Thực hiện cập nhật thông tin
            bool result = CommonDataService.UpdateCustomer(data);

            // Nếu cập nhật thất bại (ví dụ: email trùng)
            if (!result)
            {
                ModelState.AddModelError(nameof(data.Email), "Email bị trùng hoặc đã tồn tại.");
                return View("Profile", data);
            }
            // Nếu thành công, thiết lập thông báo tạm thời
            ViewBag.data = "Cập nhật thành công!";
            return View("Profile",data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            Customer data = new Customer
            {
                CustomerID = 0,
                IsLocked = false
            };

            return View(data);
        }
        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(Customer data,string pass)
        {
            ViewBag.Title = "Đăng ký";
            ViewBag.Pass = pass;
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "vui lòng nhập Email");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");
            if (string.IsNullOrWhiteSpace(pass))
                ModelState.AddModelError("pass", "Vui lòng nhập mật khẩu");
            if (!ModelState.IsValid)
            {
                return View(data);
            }
            int id = CommonDataService.CreateCustomer(data,pass);
            if (id < 0)
            {
                ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                return View(data);
            }
            return RedirectToAction("Login","Account");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var data = User.GetUserData();
            if (data == null)
            {
                return RedirectToAction("Login");
            }
            
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Mật khẩu mới không được để trống");
            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu mới không trúng khớp");
            if (!ModelState.IsValid)
                return View();
            bool result = UserAccountService.ChancePasswordCustomer(data.UserName, oldPassword, newPassword);
            if (!result)
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
            if (!ModelState.IsValid)
                return View();
            return RedirectToAction("Login");
        }
    }
}
