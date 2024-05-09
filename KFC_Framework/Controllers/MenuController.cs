using KFC_Framework.Dao;
using KFC_Framework.DTO;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Controllers
{
    public class MenuController : Controller
    {
        private ProductDao productDao = new ProductDao();

        [HttpGet]
        public ActionResult Index()
        {
            List<SanPham> sanPhams = productDao.findAllSanPham();

            var listSanPhamGroupBy = productDao.GroupProductsByCategory(sanPhams);

            return View(listSanPhamGroupBy);    
        }

        public ActionResult Detail_menu(string id, string lh)
        {
            var product = productDao.getProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            @ViewBag.lh= productDao.GetTenLHByMaSPMaLH(id, lh);
            return View(product);
        }

        public ActionResult AddToCart(string id)
        {
            var product = productDao.getProductById(id);
            if (product != null)
            {
                // Lấy giỏ hàng từ session
                var cart = Session["Cart"] as List<CartItem>;

                if (cart == null)
                {
                    // Nếu giỏ hàng chưa tồn tại, tạo mới
                    cart = new List<CartItem>();
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingItem = cart.FirstOrDefault(item => item.ProductId == id);

                if (existingItem != null)
                {
                    // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng lên 1
                    existingItem.Quantity++;
                }
                else
                {
                    // Nếu sản phẩm chưa tồn tại trong giỏ hàng, thêm mới với số lượng là 1
                    cart.Add(new CartItem { ProductId = id, Quantity = 1 });
                }

                // Cập nhật session với giỏ hàng mới
                Session["Cart"] = cart;
            }
            // Chuyển hướng đến trang giỏ hàng hoặc trang chi tiết sản phẩm
            return RedirectToAction("Index", "Cart"); // Chuyển hướng đến trang giỏ hàng
        }


    }
}