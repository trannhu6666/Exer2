using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security; // Bắt buộc phải có để dùng FormsAuthentication
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class LoginController : Controller
    {
        private SaleManagementDBEntities db = new SaleManagementDBEntities();

        // 1. Hàm hiển thị form Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // 2. Hàm xử lý khi bấm nút Login
        [HttpPost]
        public ActionResult Index(string email, string password)
        {
            // Dọn dẹp khoảng trắng rác lỡ gõ nhầm trên form
            var inputEmail = email != null ? email.Trim() : "";
            var inputPass = password != null ? password.Trim() : "";

            // Tìm trong bảng Users xem có tài khoản nào khớp không (Kèm Trim Database)
            var account = db.Users.FirstOrDefault(u => u.email.Trim() == inputEmail && u.password.Trim() == inputPass);

            if (account != null)
            {
                // THIẾU SÓT LỚN NHẤT ĐÃ ĐƯỢC BÙ ĐẮP Ở ĐÂY: Cấp vé đăng nhập chính thức của MVC
                FormsAuthentication.SetAuthCookie(account.email.Trim(), false);

                // Lưu tên user vào phiên làm việc (Để hiển thị ra chữ "Chào, Admin" trên giao diện)
                Session["User"] = account.email.Trim();

                return RedirectToAction("Index", "Home"); // Bay thẳng vào trang chủ
            }
            else
            {
                // Nếu account là null -> Sai tên hoặc mật khẩu
                ViewBag.Error = "Invalid Email or Password!";
                return View();
            }
        }

        // 3. Hàm Đăng xuất
        public ActionResult Logout()
        {
            // Xóa thẻ đăng nhập của MVC cũ
            FormsAuthentication.SignOut();

            // Xóa luôn Session 
            Session.Clear();

            // Đá về trang Đăng nhập
            return RedirectToAction("Index", "Login");
        }
    }
}