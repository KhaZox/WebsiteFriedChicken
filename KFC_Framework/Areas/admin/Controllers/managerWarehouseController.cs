using KFC_Framework.Dao;
using KFC_Framework.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class managerWarehouseController : Controller
    {
        ProductDao productDao= new ProductDao();
        // GET: admin/managerWarehouse
        public ActionResult Index()
        {
            //ViewBag.GiamGiaList = new SelectList(KFCEntitiess.NguyenVLs, "MaNVL", "TenNVL");
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên kho" || Session["UserRole"].ToString() == "Admin"))
            {
                var list = productDao.listWarehouse();
                return View(list);
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
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên kho" || Session["UserRole"].ToString() == "Admin"))
            {
                maKho_maNVL();

                return View();
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Create(ChiTietKho model)
        {
            productDao.addWarehouse(model);
            maKho_maNVL();
            return RedirectToAction("Index");
        }
        private void maKho_maNVL()
        {
            List<Kho> typeWarehouse = productDao.getMaKho();
            ViewData["MaKho"] = typeWarehouse;


            List<NguyenVL> typeParts = productDao.getNguyenVLs();
            ViewData["MaNVL"] = typeParts;
        }

        [HttpPost]
        public ActionResult Delete(string maKho, string maNVL)
        {
            productDao.deleteWarehouse(maKho, maNVL);
            return RedirectToAction("Index", "managerWarehouse");
        }

        [HttpGet]
        public ActionResult Edit(string maKho, string maNVL)
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên kho" || Session["UserRole"].ToString() == "Admin"))
            {
                var wareHouse = productDao.editWarehouseGet(maKho, maNVL);
                if (wareHouse == null)
                {
                    return HttpNotFound();
                }
                return View(wareHouse);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChiTietKho wareHouse)
        {
            if (ModelState.IsValid)
            {
                productDao.editWarehousePost(wareHouse);
                return RedirectToAction("Index", "managerWarehouse");
            }

            return View(wareHouse);
        }

        public void ExportExcel_EPPLUS( ChiTietKho model)
        {
            var list = productDao.listWarehouse();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Mã Kho";
            Sheet.Cells["B1"].Value = "Mã Nguyên Vật Liệu";
            Sheet.Cells["C1"].Value = "Tên Nguyên Vật Liệu";
            Sheet.Cells["D1"].Value = "Đơn Vị Tính";
            Sheet.Cells["E1"].Value = "Số Lượng Tồn";
            Sheet.Cells["F1"].Value = "Địa Chỉ";
            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.MaKho;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.MaNVL;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NguyenVL.TenNVL;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.DVT;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.SoLuongT;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Kho.DiaChi;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }


    }
}