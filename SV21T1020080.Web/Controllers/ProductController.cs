using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using SV21T1020080.BusinessLayers;
using SV21T1020080.DomainModels;
using SV21T1020080.Web.AppCodes;
using SV21T1020080.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class ProductController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                };
            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "",condition.CategoryID,condition.SupplierID,condition.MinPrice,condition.MaxPrice);
           
            ProductSearchResult model = new ProductSearchResult()
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
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var data = new Product()
            {
                ProductID = 0,
                IsSelling = true,
            };

            return View("Edit", data);
        }
        public IActionResult Edit(int id=0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (data.CategoryID<=0)
                ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống");
            if (data.SupplierID <= 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (data.Price <= 0)
                ModelState.AddModelError(nameof(data.Price), "Giá phải lớn hơn 0");
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";//1 chuỗi số ticks + tên file upload
                //string folder = @"D:\CODE\LapTrinhWeb\SV21T1020080\SV21T1020080.Web\wwwroot\images\employees";
                //string filePath = Path.Combine(folder, fileName); // nối 2 chuỗi thành 1 đường dẫn dẫn đến file ảnh (thắc mắc : folder là 1 được dẫn đến emplyees + file name là số xong đến tên file )
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data); ;
        }
        public IActionResult Photo(int id)
        {
            ViewBag.Title = "Bổ sung thư viện ảnh";
            var data = new ProductPhoto()
            {
                PhotoID = 0,
                ProductID = id,
            };

            return View(data);
        }
        public IActionResult EditPhoto(int id = 0 , int productID = 0)
        {
                ViewBag.Title = "Cập nhật thông tin thư viện ảnh";
                var data = ProductDataService.GetPhoto(id);
                if (data == null)
                {
                    return RedirectToAction("Index");
                }
            else
            {
                var item = new ProductPhoto()
                {
                    PhotoID=id,
                    Photo = data.Photo,
                    ProductID = productID,
                    Description = data.Description,
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };

                return View("Photo", item);
            }
               
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data , IFormFile? uploadPhoto )
        {
            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Tên mô tả ảnh không được để trống");
            if (data.DisplayOrder<=0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị ảnh không được nhỏ hơn bằng 0");
            if (uploadPhoto == null)
                ModelState.AddModelError(nameof(data.Photo), "Vui vòng tải ảnh lên");
            if (!ModelState.IsValid)
            {
                return View("Photo", data);
            }
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";//1 chuỗi số ticks + tên file upload
                //string folder = @"D:\CODE\LapTrinhWeb\SV21T1020080\SV21T1020080.Web\wwwroot\images\employees";
                //string filePath = Path.Combine(folder, fileName); // nối 2 chuỗi thành 1 đường dẫn dẫn đến file ảnh (thắc mắc : folder là 1 được dẫn đến emplyees + file name là số xong đến tên file )
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(data);
                var item = ProductDataService.GetProduct(data.ProductID);
                if (item != null) {
                    return View("Edit", item);
                }
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(data);
                var item = ProductDataService.GetProduct(data.ProductID);
                if (item != null) {
                    return View("Edit", item);
                }
            }   
            return RedirectToAction("Index");
        }
        public IActionResult DeletePhoto(long id , int productID=0) {
            ViewBag.Title = "Xoá thư viện";
                bool result= ProductDataService.DeletePhoto(id);
                var item = ProductDataService.GetProduct(productID);
                if (item != null)
                {
                    return View("Edit", item);
                }
            return RedirectToAction("Index");
        }
        public IActionResult Attribute(int id = 0)
        {
            ViewBag.Title = "Bổ sung thuộc tính mặt hàng";
            var data = new ProductAttribute()
            {
                AttributeID = 0,
                ProductID = id,
            };
            return View(data);
        }
        public IActionResult EditAttribute(long id = 0, int productID = 0)
        {
            ViewBag.Title = "Cập nhật thông tin thuộc tính mặt hàng";
            var data = ProductDataService.GetAttribute(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var item = new ProductAttribute()
                {
                    AttributeID = id,
                    ProductID = productID,
                    AttributeName = data.AttributeName,
                    DisplayOrder = data.DisplayOrder,
                    AttributeValue = data.AttributeValue,
                };

                return View("Attribute", item);
            }

        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính mặt hàng" : "Cập nhật thuộc tính mặt hàng";
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính mặt hàng không được để trống");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "thứ tự hiển thị ảnh không được nhỏ hơn bằng 0");
            if (!ModelState.IsValid)
            {
                return View("Attribute", data);
            }
            if (data.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(data);
                var item = ProductDataService.GetProduct(data.ProductID);
                if (item != null)
                {
                    return View("Edit", item);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                var item = ProductDataService.GetProduct(data.ProductID);
                if (item != null)
                {
                    return View("Edit", item);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult DeleteAttribute(int id, int productID = 0)
        {
            ViewBag.Title = "Xoá thuộc tính mặt hàng";
            bool result = ProductDataService.DeleteAttribute(id);
            var item = ProductDataService.GetProduct(productID);
            if (item != null)
            {
                return View("Edit", item);
            }
            return RedirectToAction("Index");
        }
    }
}
