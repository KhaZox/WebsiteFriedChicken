using KFC_Framework.Dao;
using KFC_Framework.Models;
using KFC_Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class employeeAccountController : Controller
    {
        AccountDao accountDao= new AccountDao();
        EmployeeDao employeeDao= new EmployeeDao();
        KFCEntities db= new KFCEntities();
       
        // GET: admin/employeeAccount
        public ActionResult Index()
        {
            string email = Session["LoggedInUser"] as string;
            NhanVien nhanvien = accountDao.getNhanVienById(email);
            return View(nhanvien);
        }

        // POST: adin/employeeAccount
        public ActionResult updateAccountEmployee(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                string matk = Session["MaTK"] as string;
                employeeDao.updateProfile(model, matk);
                return Json(new { success = true, message = "Cập nhật thông tin thành công." });
            }
            else
            {
                return Json(new { success = false, message = "Update fail." });
            }
        }

        public ActionResult changePassword()
        {
            return View();
        }
        public ActionResult updatePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest()) // Check if the request is an AJAX call
                    return Json(new { success = false, message = "Validation failed." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            string matk = Session["MaTK"] as string;
            var user = accountDao.getAccountById(matk);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "User not found." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            // Verify current password
            if (user.MatKhau != model.CurrentPassword) // Ideally, use hashing comparison here
            {
                ModelState.AddModelError("", "The current password is incorrect.");
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "The current password is incorrect." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            // Check new password confirmation
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The new password and confirmation password do not match.");
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Passwords do not match." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            // Update the password
            user.MatKhau = model.NewPassword; // You should hash this password
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Password successfully changed." }, JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
    }
}