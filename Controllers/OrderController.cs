using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAppMVC.Models; // Đổi thành namespace Models của bạn

namespace WebAppMVC.Controllers
{
    public class OrderController : Controller
    {
        private SaleManagementDBEntities db = new SaleManagementDBEntities();
        
        // Lớp giả để chứa giỏ hàng (Giống WinForm)
        public class CartItem
        {
            public int ItemID { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Total => Quantity * UnitPrice;
        }

        // 1. MÀN HÌNH TẠO ĐƠN HÀNG
        public ActionResult Create()
        {
            // Truyền danh sách Agent và Item sang View để làm Dropdown
            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemName");

            // Lấy giỏ hàng từ Session ra hiển thị
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }

        // 2. NÚT ADD TO CART (Dùng Ajax hoặc Form Submit)
        [HttpPost]
        // 1. Thêm dấu chấm hỏi (?) vào sau chữ decimal để cho phép nhận null không bị crash
        public ActionResult AddToCart(int ItemID, int Quantity, decimal? UnitPrice)
        {
            // 2. Chặn lỗi: Nếu người dùng không nhập hoặc nhập sai chữ/dấu phẩy
            if (!UnitPrice.HasValue)
            {
                TempData["Error"] = "Vui lòng nhập đúng Giá tiền (chỉ nhập số, không nhập dấu phẩy hay chữ)!";
                return RedirectToAction("Create");
            }

            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var item = db.Items.Find(ItemID);

            if (item != null)
            {
                var existingItem = cart.FirstOrDefault(c => c.ItemID == ItemID);
                if (existingItem != null)
                {
                    existingItem.Quantity += Quantity; // Cộng dồn số lượng
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ItemID = item.ItemID,
                        ItemName = item.ItemName,
                        Quantity = Quantity,
                        UnitPrice = UnitPrice.Value // .Value vì bây giờ nó là kiểu cho phép null
                    });
                }
                Session["Cart"] = cart;
                TempData["Success"] = "Đã thêm món vào giỏ hàng!";
            }

            return RedirectToAction("Create");
        }

        // 3. NÚT SAVE ORDER (Lưu vào DB)
        [HttpPost]
        public ActionResult SaveOrder(int AgentID, DateTime OrderDate)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Cart is empty!";
                return RedirectToAction("Create");
            }

            // Tạo Order mới
            Order newOrder = new Order
            {
                AgentID = AgentID,
                OrderDate = OrderDate
            };

            // Thêm chi tiết đơn hàng
            foreach (var cartItem in cart)
            {
                newOrder.OrderDetails.Add(new OrderDetail
                {
                    ItemID = cartItem.ItemID,
                    Quantity = cartItem.Quantity,
                    UnitAmount = cartItem.UnitPrice
                });
            }

            db.Orders.Add(newOrder);
            db.SaveChanges(); // Lưu cái rụp xuống DB

            Session["Cart"] = null; // Xóa giỏ hàng
            TempData["Success"] = "Order Saved Successfully! Order ID: " + newOrder.OrderID;

            return RedirectToAction("Create");
        }
    }
}