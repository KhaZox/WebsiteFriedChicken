using KFC_Framework.Dao;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class managerOrderController : Controller
    {
        private OrderDao orderDao = new OrderDao();
        private EmployeeDao employeeDao = new EmployeeDao();
        // GET: admin/managerOrder
        public ActionResult Index()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                var list = orderDao.getListOrders();
                return View(list);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        ////Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                DonDatHang ddh = orderDao.getByIdOrder(id);
                return View(ddh);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        //[HttpPost]
        [HttpPost]
        public ActionResult Edit(DonDatHang model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    orderDao.editOrder(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult exportBill(string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DonDatHang donDatHang = orderDao.getByIdOrder(id);
                    List<CTDonDatHang> cTDonDatHang = orderDao.getDetailOrderById(id);
                    string matk = Session["MaTK"] as string;
                    string manv = employeeDao.getByMaTK(matk).MaNV;
                    orderDao.InsertBillInfo(donDatHang,cTDonDatHang, manv);
                    return RedirectToAction("Index");
                }
                return View(id);
            }
            catch
            {
                return View();
            }
        }
    }
}
