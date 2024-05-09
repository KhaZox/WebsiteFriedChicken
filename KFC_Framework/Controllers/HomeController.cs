using KFC_Framework.Dao;
using KFC_Framework.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Controllers
{
    public class HomeController : Controller
    {
        private ProductDao homeDao = new ProductDao();
        private OrderDao orderDao = new OrderDao();
        private readonly KFCEntities db= new KFCEntities();
        // GET: home
        public ActionResult Index()
        {
            // Retrieve data from your database
            List<SanPham> sanPhamList = homeDao.GetSanPhamListByMaLH("LH004");

            // Pass the data to the view
            return View(sanPhamList);
        }

        public ActionResult FollowOrder()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getOrder(string maddh)
        {
            using (var db = new KFCEntities())
            {
                var orderDetails = db.DonDatHangs.FirstOrDefault(o => o.MaDDH == maddh);

                if (orderDetails != null)
                {
                    var query = db.DonDatHangs
                                 .Where(x => x.MaDDH == maddh)
                                 .Select(x => new
                                 {
                                     x.MaDDH,
                                     x.MaKH,
                                     x.TenNguoiNhan,
                                     x.TriGia,
                                     x.NgayDH,
                                     x.NgayGH,
                                     x.TinhTrang
                                 })
                                 .FirstOrDefault(); 

                    return Json(query);
                }
                else
                {
                    return Json(new { error = "Order not found" });
                }
            }
        }
        
        [HttpPost]
        public ActionResult deleOrder(string maddh)
        {
            try
            {
                var idOrder = orderDao.getByIdOrder(maddh);
                if (idOrder != null)
                {
                    orderDao.delOrder_DetailOrder(maddh);
                }
                return RedirectToAction("FollowOrder");

            }
            catch
            {
                return View();
            }
            
        }

    }
}