using System.Linq;
using System.Web.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class ReportController : Controller
    {
        private SaleManagementDBEntities db = new SaleManagementDBEntities();

        public ActionResult PrintOrder()
        {
            // Tìm cái đơn hàng vừa mới được thêm vào DB (OrderID lớn nhất/mới nhất)
            var latestOrder = db.Orders.OrderByDescending(o => o.OrderID).FirstOrDefault();

            if (latestOrder == null)
            {
                return Content("Không có đơn hàng nào để in!");
            }

            // Trả cục dữ liệu của đơn hàng đó ra giao diện để in
            return View(latestOrder);
        }
    }
}