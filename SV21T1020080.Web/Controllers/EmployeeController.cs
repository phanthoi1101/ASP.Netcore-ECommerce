using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using SV21T1020080.Web.AppCodes;
using SV21T1020080.Web.Models;
using System.Globalization;

namespace SV21T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN}")]
    public class EmployeeController : Controller
    {
        public const int PAGE_SIZE = 16;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Nhân viên";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Employee data,string _birthDate, IFormFile? uploadPhoto)///gửi lên 1 hình ảnh IFormFile
        {

            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(_birthDate))
                ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh của nhân viên");
           
            //Xử lý ngày sinh
            DateTime? d = _birthDate.ToDateTime();
            
            if (d != null && d > (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue && d < (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue && d < DateTime.Now)
            {
                data.BirthDate = d.Value;
            }
            else
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không chính xác");


            if (string.IsNullOrWhiteSpace(data.Address))
                data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone))
                data.Address = "";
            
            if (!ModelState.IsValid)
            {
                return View("Edit",data);
            }

            
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";//1 chuỗi số ticks + tên file upload
                //string folder = @"D:\CODE\LapTrinhWeb\SV21T1020080\SV21T1020080.Web\wwwroot\images\employees";
                //string filePath = Path.Combine(folder, fileName); // nối 2 chuỗi thành 1 đường dẫn dẫn đến file ảnh (thắc mắc : folder là 1 được dẫn đến emplyees + file name là số xong đến tên file )
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream); 
                }
                data.Photo = fileName;
            }

            if (data.EmployeeID == 0)
            {
                int id = CommonDataService.AddEmployee(data);
                if (id <= 0) 
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
               bool result =  CommonDataService.UpdateEmployee(data);
                if (!result) 
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }

}