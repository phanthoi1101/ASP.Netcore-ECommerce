using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020080.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string username , string password)
        {
            ViewBag.UserName = username;

            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }
            //Kiểm tra xem user có đúng hay không
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password); 
            if(userAccount == null)
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
           await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,userData.CreatePrincipal());
           //Quay về trang chủ
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");    
        }
         
        public IActionResult ChangePassword(string oldPassword, string newPassword,string confirmPassword)
        {
            var data = User.GetUserData();
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Mật khẩu mới không được để trống");
            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu mới không trúng khớp");
            if (!ModelState.IsValid)
                return View();
            bool result = UserAccountService.ChancePassword(data.UserName, oldPassword, newPassword);
            if (!result)
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
            if (!ModelState.IsValid)
                return View();
            return RedirectToAction("Login");
        }
        public IActionResult ForgotPassword() {
            return View();  
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
    }

}
