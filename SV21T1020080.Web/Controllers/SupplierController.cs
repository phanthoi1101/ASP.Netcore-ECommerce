using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using SV21T1020080.Web.AppCodes;
using SV21T1020080.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class SupplierController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition) {
            int rowCount;
            var data = CommonDataService.ListOfSupliers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            SupplierSearchResult model = new SupplierSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Edit( int id =0)
        { 
            ViewBag.title = "Edit nhà cung cấp";
            var data = CommonDataService.GetSuplier(id);
            if (data == null)
            {
                return View("Index");
            }
            return View(data);
        }

        public IActionResult Create() {
            var data = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", data);
        }

        [HttpPost]
        public IActionResult Save(Supplier item)
        {
            ViewBag.Title = item.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";
            if (string.IsNullOrWhiteSpace(item.SupplierName))
                ModelState.AddModelError(nameof(item.SupplierName), "Tên nhà cung cấp không được để trống");
            if (string.IsNullOrWhiteSpace(item.ContactName))
                ModelState.AddModelError(nameof(item.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(item.Phone))
                ModelState.AddModelError(nameof(item.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(item.Email))
                ModelState.AddModelError(nameof(item.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(item.Address))
                ModelState.AddModelError(nameof(item.Address), "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(item.Provice))
                ModelState.AddModelError(nameof(item.Provice), "Vui lòng chọn tỉnh thành");


            if (!ModelState.IsValid)
            {
                return View("Edit", item);
            }

            if (item.SupplierID == 0)
            {
                
                int id = CommonDataService.AddSupplier(item);
                if(id < 0)
                {
                    ModelState.AddModelError(nameof(item.Email), "Email bị trùng");
                    return View("Edit", item);
                }
            }
            else
            {
                
                bool result = CommonDataService.UpdateSupplier(item);
                if (!result)
                {
                    ModelState.AddModelError(nameof(item.Email), "Email bị trùng");
                    return View("Edit", item);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) {
            ViewBag.title = "Xoá nhà cung cấp";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetSuplier(id);
            if (data == null)
            {
                return View("Index");
            }
            return View(data);
        }
    }
}
