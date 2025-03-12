using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DataLayers
{
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy dánh sách dữ liệu dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">số dòng trong 1 trang (bằng 0 tức là không có phần trang)</param>
        /// <param name="searchValue">Giá trị cần tìm kiếm ( chuỗi rỗng tức là lấy toàn bộ</param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm kiếm được
        /// </summary>
        /// <param name="searchValue">giá trị cần tìm kiếm (nếu là chuỗi rỗng thì đếm toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue);
        /// <summary>
        /// hàm lấy về 1 dòng dữ liệu dựa trên khoá chính
        /// </summary>
        /// <param name="id"> khoá chính của của dữ liệu</param>
        /// <returns></returns>
       T? Get(int id);
        /// <summary>
        /// hàm bổ sung 1 bản ghi vào cơ sử dữ liệu và trả về khoá chính của đúng bản ghi đó
        /// </summary>
        /// <param name="item"> đưa vào 1 đối tượng cần bổ sung</param>
        /// <returns></returns>
        int Add(T item);
        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);

        /// <summary>
        /// Xóa 1 dòng dữ liệu dựa vào giá trị của khóa chính/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// Kiểm tra xem 1 dòng dữ liệu có khóa là id hiện có dữ liệu tham chiếu hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);

        int Create(T data, string pass);

    }
}
