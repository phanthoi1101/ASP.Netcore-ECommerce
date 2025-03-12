

namespace SV21T1020080.DataLayers
{
    public interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
