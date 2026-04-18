using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            // Tìm trong bảng Users xem có tài khoản nào khớp cả Email và Password không
            // Lưu ý: Chữ "Users" là tên bảng, "Email" và "Password" là tên cột trong DB của bạn.
            var account = db.Users.FirstOrDefault(u => u.email == email && u.password == password);

            if (account != null)
            {
                // Nếu tìm thấy -> Đăng nhập thành công
                Session["User"] = account.email; // Lưu tên user vào phiên làm việc
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
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}