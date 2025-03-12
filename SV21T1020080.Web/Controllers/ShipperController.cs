using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using SV21T1020080.Web.AppCodes;
using SV21T1020080.Web.Models;
using System.Xml.Linq;

namespace SV21T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create(int id = 0  )
        {
            ViewBag.title = "Bổ sung người giao hàng";
            var data = new Shipper()
            {
                ShipperID = id,
            };
            return View("Edit",data);
        }
        public IActionResult Edit(int id = 0)
        {
            var data = CommonDataService.GetShipper(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Delete(int id)
        {
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Shipper shipper)
        {
            ViewBag.Title = shipper.ShipperID == 0 ? "Bổ sung Shipper" : "Cập nhật thông tin Shipper";
            if (string.IsNullOrWhiteSpace(shipper.ShipperName))
                ModelState.AddModelError(nameof(shipper.ShipperName), "Tên Shipper được để trống");
            if (string.IsNullOrWhiteSpace(shipper.Phone))
                ModelState.AddModelError(nameof(shipper.Phone), "Số điện thoại được để trống");
            if (!ModelState.IsValid)
            {
                return View("Edit", shipper);
            }
            if (shipper.ShipperID==0)
            {
               int id =  CommonDataService.AddShipper(shipper);
                if (id < 0)
                {
                    ModelState.AddModelError(nameof(shipper.Phone), "Trùng số điện thoại");
                    return View("Edit", shipper);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateShipper(shipper);
                if (!result)
                {
                    ModelState.AddModelError(nameof(shipper.Phone), "Trùng số điện thoại");
                    return View("Edit", shipper);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
