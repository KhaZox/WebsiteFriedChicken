using KFC_Framework.Dao;
using KFC_Framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using KFC_Framework.ViewModels;
using System.Data.Entity;
using System.Runtime.Serialization;
using OfficeOpenXml;
using static KFC_Framework.Dao.ReportDao;

namespace KFC_Framework.Areas.admin.Controllers
{
    public class managerReportController : Controller
    {
        private ReportDao reportDao = new ReportDao();
        private OrderDao orderDao = new OrderDao();
        private KFCEntities db = new KFCEntities();

        // GET: admin/manageReport
        public ActionResult salesReport()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                var hd = reportDao.findAllBill();
                return View(hd);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult GetChartData()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                var hoaDonData = db.HoaDons.ToList();
                var labels = new List<string>();
                var data = new List<double>();
                foreach (var hoaDon in hoaDonData)
                {
                    string month = hoaDon.NgayBan.ToString("dd/MM/yyyy");
                    labels.Add(month);
                    data.Add(hoaDon.TriGia);
                }

                return Json(new { labels = labels, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }


        public ActionResult GetSalesData(int month, int year)
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                var salesDataList = db.HoaDons
                .Where(s => s.NgayBan.Month == month && s.NgayBan.Year == year)
                .GroupBy(s => s.NgayBan.Month)
                .Select(g => new { Month = g.Key, Revenue = g.Sum(s => s.TriGia) })
                .ToList();

                // Return sales data as JSON
                return Json(salesDataList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        // GET: admin/manageReport/ordersReport
        [HttpGet]
        public ActionResult ordersReport()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Nhân viên" || Session["UserRole"].ToString() == "Admin"))
            {
                string ordersReportJson = reportDao.getOrderChart();
                ViewBag.DataPoints = ordersReportJson;
                return View();
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }


        public void ExportExcel_EPPLUS(DonDatHang model)
        {
            var list = orderDao.getListOrders();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Mã Đơn Đặt Hàng";
            Sheet.Cells["B1"].Value = "Mã Khách Hàng";
            Sheet.Cells["C1"].Value = "Ngày Đặt Hàng";
            Sheet.Cells["D1"].Value = "Ngày Giao Hàng";
            Sheet.Cells["E1"].Value = "Tên Người Nhận";
            Sheet.Cells["F1"].Value = "Địa Chỉ Nhận";
            Sheet.Cells["G1"].Value = "Số Điện Thoại Nhận Hàng";
            Sheet.Cells["H1"].Value = "Email";
            Sheet.Cells["I1"].Value = "Hình Thức Thanh Toán";
            Sheet.Cells["J1"].Value = "Hình Thức Giao Hàng";
            Sheet.Cells["K1"].Value = "Trị Giá";
            Sheet.Cells["L1"].Value = "Loại Khách Hàng";
            Sheet.Cells["M1"].Value = "Tình Trạng";
            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.MaDDH;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.MaKH;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NgayDH;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.NgayGH;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.TenNguoiNhan;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.DiaChiNhan;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.SDTNhanHang;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.HTThanhToan;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.HTGiaoHang;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.TriGia;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.LoaiKhachHang;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.TinhTrang;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "ReportOrders.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
    }
}