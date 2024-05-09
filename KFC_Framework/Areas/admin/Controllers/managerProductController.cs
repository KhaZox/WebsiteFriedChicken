using KFC_Framework.Dao;
using KFC_Framework.Models;
using KFC_Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class managerProductController : Controller
    {
        KFCEntities KFCEntitiess= new KFCEntities();

        private ProductDao productDao = new ProductDao();

        public ActionResult Index()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                var products = productDao.findAllSanPham();
                return View(products);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                List<LoaiHang> typeProducts = productDao.getTenLoaiHang();
                ViewData["TypeProducts"] = typeProducts;

                List<GiamGia> giamGiaList = productDao.getMaGiamGia();
                ViewData["GiamGiaList"] = giamGiaList;

                return View();
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham model, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (HinhAnh != null && HinhAnh.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(HinhAnh.FileName);
                        string uploadPath = Server.MapPath("~/Upload/");
                        string filePath = Path.Combine(uploadPath, fileName);
                        HinhAnh.SaveAs(filePath);

                        model.HinhAnh = "~/Upload/" + fileName;
                    }
                    KFCEntitiess.SanPhams.Add(model);
                    KFCEntitiess.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "There was an error while creating the product: " + ex.Message);
                }
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(SanPham model, HttpPostedFileBase HinhAnh)
        {
            var updateModel = KFCEntitiess.SanPhams.Find(model.MaSP);

            if (updateModel != null)
            {
                updateModel.MaLH = model.MaLH;
                updateModel.MaGG = model.MaGG;
                updateModel.TenSP = model.TenSP;
                updateModel.GiaBanSP = model.GiaBanSP;
                updateModel.MoTaSP = model.MoTaSP;

                if (HinhAnh != null && HinhAnh.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(HinhAnh.FileName);
                    string uploadPath = Server.MapPath("~/Upload/");
                    string filePath = Path.Combine(uploadPath, fileName);
                    HinhAnh.SaveAs(filePath);

                    updateModel.HinhAnh = "~/Upload/" + fileName;
                }

                KFCEntitiess.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
        }

        //Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                ViewBag.LoaiHangList = new SelectList(KFCEntitiess.LoaiHangs, "MaLH", "TenLH");
                ViewBag.GiamGiaList = new SelectList(KFCEntitiess.GiamGias, "MaGG", "TenSuKien");

                //SanPham sanPham = KFCEntitiess.SanPhams.Find(id);
                SanPham sanPham = productDao.getProductById(id);
                return View(sanPham);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var product = productDao.getProductById(id);    
            if (product != null)
            {
                productDao.deleteProduct(id);
            }
            else
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

    }
}