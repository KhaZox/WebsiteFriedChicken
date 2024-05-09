using KFC_Framework.Dao;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class managerDiscountController : Controller
    {
        KFCEntities KFCEntitiess = new KFCEntities();
        private DiscountDao discountDao = new DiscountDao();
        // GET: admin/managerDiscount
        public ActionResult Index()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                var list = discountDao.getListDiscount();
                return View(list);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        // GET: admin/managerDiscount/Create
        public ActionResult Create()
        {
            if (Session["UserRole"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        // POST: admin/managerDiscount/Create
        [HttpPost]
        public ActionResult Create(GiamGia model)
        {
            try
            {
                discountDao.addDiscount(model);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: admin/managerDiscount/Edit/5
        public ActionResult Edit(string id)
        {
            if (Session["UserRole"].ToString() == "Admin")
            {
                GiamGia getDiscount = discountDao.getByIdDiscount(id);
                return View(getDiscount);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }

        }

        // POST: admin/managerDiscount/Edit/5
        [HttpPost]
        public ActionResult Edit(GiamGia model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    discountDao.editDiscount(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // POST: admin/managerDiscount/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
                {
                    var discount = discountDao.getByIdDiscount(id);
                    if (discount != null)
                    {
                        discountDao.delDiscount(id);
                    }
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
