using SV21T1020080.DataLayers;
using SV21T1020080.DataLayers.SQLServer;
using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;
        static ProductDataService() {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        public static List<Product> ListProducts(string searchValue = "")
        {
            return null;
        }
        public static List<Product> ListProducts(out int rowCount,int page=1,int pageSize = 0,string searchValue="",int categoryId = 0 , int supplierId=0, decimal minPrice=0,decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue,categoryId,supplierId,minPrice,maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice);
        }
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        public static int AddProduct(Product product)
        {
            return productDB.Add(product);
        }
        public static bool UpdateProduct(Product product)
        {
            return productDB.Update(product);
        }
        public static bool DeleteProduct(int productID)
        {
            return productDB.Delete(productID);
        }
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }
        public static IList<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID);
        }
        public static ProductPhoto? GetPhoto(long PhotoID)
        {
            return productDB.GetPhoto(PhotoID);
        }
        public static long AddPhoto(ProductPhoto photo)
        {
            return productDB.AddPhoto(photo);
        }
        public static bool UpdatePhoto(ProductPhoto photo)
        {
            return productDB.UpdatePhoto(photo);
        }
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }
        public static IList<ProductAttribute> ListProductAttributes(int productID)
        {
            return productDB.ListAttributes(productID);
        }
        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        public static long AddAttribute(ProductAttribute attribute)
        {
            return productDB.AddAttribute(attribute);
        }
        public static bool UpdateAttribute(ProductAttribute attribute)
        {
            return productDB.UpdateAttribute(attribute);
        }
        public static bool DeleteAttribute(int productID)
        {
            return productDB.DeleteAttribute(productID);
        }
        public static ProductPhoto? GetPhoto_productID(int productID)
        {
            return productDB.GetPhoto_productID(productID);
        }
        public static ProductAttribute? GetAttribute_productID(int productID)
        {
            return productDB.GetAttribute_productID(productID);
        }

    }
}
