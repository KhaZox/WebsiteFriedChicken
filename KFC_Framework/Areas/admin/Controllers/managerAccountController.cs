using KFC_Framework.Dao;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class managerAccountController : Controller
    {
        AccountDao accountDao = new AccountDao();
        // GET: admin/managerAccount
        public ActionResult Index()
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                var list = accountDao.getAllAccount();
                return View(list);
            } else 
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                var delAccount = accountDao.getByIdAccount(id);
                if (delAccount != null)
                {
                    accountDao.delAccount(id);
                    return RedirectToAction("Index", "managerAccount");
                }
                else
                {
                    return RedirectToAction("NotAuthorized", "Error"); // Redirect to a not authorized page
                }
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        //Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                TaiKhoan account = accountDao.getByIdAccount(id);
                return View(account);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Edit(TaiKhoan model)
        {
            accountDao.editAccount(model);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                return View("Create");
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
        [HttpPost]
        public ActionResult Create(TaiKhoan model)
        {
            accountDao.addAccount(model);
            return RedirectToAction("Index");
        }
    }
}