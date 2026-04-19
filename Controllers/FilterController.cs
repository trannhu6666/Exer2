using System.Data.Entity; // Rất quan trọng để dùng được hàm .Include()
using System.Linq;
using System.Web.Mvc;
using WebAppMVC.Models; // Nhớ đổi tên chỗ này

namespace WebAppMVC.Controllers
{
    public class FilterController : Controller
    {
        // Khởi tạo kết nối Database (Đổi tên cho đúng với file .edmx của bạn)
        private SaleManagementDBEntities db = new SaleManagementDBEntities();

        // Biến tham số nhận vào từ URL khi người dùng bấm nút Lọc
        public ActionResult Index(int? searchAgentId, int? searchItemId)
        {
            // 1. Gửi dữ liệu đổ vào 2 cái Dropdown List (có giữ lại giá trị vừa chọn)
            ViewBag.searchAgentId = new SelectList(db.Agents, "AgentID", "AgentName", searchAgentId);
            ViewBag.searchItemId = new SelectList(db.Items, "ItemID", "ItemName", searchItemId);

            var bestItemsDict = db.OrderDetails
                .Where(od => od.Item != null && od.Quantity != null) // Bỏ qua dữ liệu rỗng
                .GroupBy(od => od.Item.ItemName)                     // Nhóm theo tên mặt hàng
                .ToDictionary(
                    g => g.Key,                                      // Tên mặt hàng
                    g => g.Sum(od => od.Quantity ?? 0)               // Tổng số lượng bán ra
                )
                .OrderByDescending(kvp => kvp.Value)                 // Sắp xếp giảm dần theo số lượng
                .Take(3)                                             // Lấy Top 3
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Đóng gói gửi sang View
            ViewBag.BestItems = bestItemsDict;
            // 2. Viết câu Query gốc, dùng Include để nối các bảng lại với nhau
            var query = db.OrderDetails
                          .Include(od => od.Order)
                          .Include(od => od.Order.Agent)
                          .Include(od => od.Item)
                          .AsQueryable();

            // 3. Nếu có chọn Đại lý thì lọc theo Đại lý
            if (searchAgentId.HasValue)
            {
                query = query.Where(od => od.Order.AgentID == searchAgentId.Value);
            }

            // 4. Nếu có chọn Sản phẩm thì lọc theo Sản phẩm
            if (searchItemId.HasValue)
            {
                query = query.Where(od => od.ItemID == searchItemId.Value);
            }

            // 5. Thực thi câu lệnh, sắp xếp ngày mới nhất lên đầu và ném sang View
            var results = query.OrderByDescending(od => od.Order.OrderDate).ToList();

            return View(results);
        }
    }
}