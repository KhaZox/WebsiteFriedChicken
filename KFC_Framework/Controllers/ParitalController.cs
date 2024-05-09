using KFC_Framework.Dao;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Controllers
{
    public class ParitalController : Controller
    {
        private AccountDao accountDao = new AccountDao();
        private CustomerDao customerDao = new CustomerDao();
        private OrderDao orderDao = new OrderDao();

        // GET: Parital
        public ActionResult _PartialHeaderSub()
        {
            ViewBag.text = "Đặt hàng";
            ViewBag.takeAway1 = "Giao hàng";
            ViewBag.takeAway2 = "hoặc mang đi";
            return PartialView();
        }

        public ActionResult _PartialBanner()
        {
            return PartialView();
        }
        
        public ActionResult _PartialSlider()
        {
            ViewBag.slider1 = "https://static.kfcvietnam.com.vn/images/content/home/carousel/lg/GaDiaTrungHai.webp?v=LE5pPL";
            ViewBag.Slider2 = "https://static.kfcvietnam.com.vn/images/content/home/carousel/lg/MiY.webp?v=LAw0Ag";
            ViewBag.slider3 = "https://static.kfcvietnam.com.vn/images/content/home/carousel/lg/GaQueKem.jpg?v=LE5pPL";
            ViewBag.slider4 = "https://static.kfcvietnam.com.vn/images/content/home/carousel/lg/GaCuonBK.jpg?v=LE5pPL";
            return PartialView();
        }
        
        public ActionResult _PartialMenuItems()
        {
            return PartialView();
        }
        
        public ActionResult _PartialSocialFooter()
        {
            return PartialView();
        }
        
        public ActionResult _PartialReset_Password()
        {
            return PartialView();
        }
        
        public ActionResult _PartialDeletaAccount()
        {
            return PartialView();
        }
        
        public ActionResult _PartialEmtyCart()
        {
            return PartialView();
        }
        
        public ActionResult _PartialErrorAccessRole()
        {
            return PartialView();
        }

        public ActionResult _PartialHistoryOrder()
        {
            string matk = Session["MaTK"] as string;
            string makh = customerDao.getByID(matk).MaKH;
            List<DonDatHang> donDatHang = orderDao.getListOrdersByMakh(makh);
            return PartialView(donDatHang);
        }
        
        public ActionResult _PartialDetailAccount()
        {
            //string email = Session["LoggedInUser"] as string;
            //KhachHang khachHang = accountDao.getKhachHangById(email);
            string matk = Session["MaTK"] as string;
            KhachHang khachHang = accountDao.getKhachHangById(matk);
            return PartialView(khachHang);
        }

        public ActionResult _PartialAccount()
        {
            string email = Session["LoggedInUser"] as string;
            ViewBag.UserEmail = email;
            return PartialView();
        }

        public ActionResult _PartialLocalCustomer()
        {
            string matk = Session["MaTK"] as string;
            string makh = customerDao.getByID(matk).MaKH;
            List<DonDatHang> donDatHang = orderDao.getListOrdersByMakh(makh);
            return PartialView(donDatHang);
        }

    }
}