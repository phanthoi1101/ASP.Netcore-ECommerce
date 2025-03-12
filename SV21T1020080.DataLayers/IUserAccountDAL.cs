using SV21T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Kiểm tra xem tên đăng nhập và mật khẩu có đúng hay không?
        /// Nếu đúng: trả về thông tin User, nếu sai trả về null
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username, string password);
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(string username, string oldPassword, string newPassword);
    }
}
