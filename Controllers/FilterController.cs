using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAppMVC.Models; // Đổi lại thành namespace Models của bạn nhé

namespace WebAppMVC.Controllers
{
    public class FilterController : Controller
    {
        private SaleManagementDBEntities db = new SaleManagementDBEntities();

        // Class phụ để chứa dữ liệu in ra bảng HTML
        public class ReportViewModel
        {
            public DateTime? Date { get; set; }
            public string Name { get; set; }

            // ĐÃ THÊM DẤU CHẤM HỎI VÀO ĐÂY GIÚP BẠN:
            public int? Quantity { get; set; }
        }

        // 1. Hàm Load trang đầu tiên (Chỉ hiện Dropdown, chưa có bảng)
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemName");
            return View();
        }

        // 2. Hàm xử lý khi người dùng bấm các nút Lọc
        [HttpPost]
        public ActionResult Index(string actionType, int? AgentID, int? ItemID)
        {
            // Nạp lại dữ liệu cho 2 cái Dropdown để nó không bị biến mất
            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemName");

            List<ReportViewModel> result = new List<ReportViewModel>();

            // Lọc 1: Mặt hàng bán chạy nhất
            if (actionType == "BestSelling")
            {
                ViewBag.ReportTitle = "Best Selling Items";
                result = (from od in db.OrderDetails
                          group od by od.Item.ItemName into g
                          select new ReportViewModel
                          {
                              Name = g.Key,
                              Quantity = g.Sum(x => x.Quantity)
                          }).OrderByDescending(x => x.Quantity).ToList();
            }
            // Lọc 2: Món hàng được mua bởi 1 Đại lý cụ thể
            else if (actionType == "ByAgent" && AgentID.HasValue)
            {
                ViewBag.ReportTitle = "Items Purchased By Selected Agent";
                result = (from o in db.Orders
                          join od in db.OrderDetails on o.OrderID equals od.OrderID
                          where o.AgentID == AgentID.Value
                          select new ReportViewModel
                          {
                              Date = o.OrderDate,
                              Name = od.Item.ItemName, // Hiện tên món
                              Quantity = od.Quantity
                          }).ToList();
            }
            // Lọc 3: Khách hàng đã mua 1 món cụ thể
            else if (actionType == "ByItem" && ItemID.HasValue)
            {
                ViewBag.ReportTitle = "Agents Who Purchased Selected Item";
                result = (from o in db.Orders
                          join od in db.OrderDetails on o.OrderID equals od.OrderID
                          where od.ItemID == ItemID.Value
                          select new ReportViewModel
                          {
                              Date = o.OrderDate,
                              Name = o.Agent.AgentName, // Hiện tên khách
                              Quantity = od.Quantity
                          }).ToList();
            }

            // Trả kết quả ra màn hình
            return View(result);
        }
    }
}