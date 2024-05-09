using KFC_Framework.Dao;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KFC_Framework.ViewModels;
using KFC_Framework.DTO;

namespace KFC_Framework.Controllers
{
    public class CartController : Controller
    {
        private ProductDao productDao = new ProductDao();
        private DiscountDao discountDao = new DiscountDao();
        // GET: Cart
        public ActionResult Index()
        {
            // Lấy danh sách mục giỏ hàng từ session
            var cartItems = Session["Cart"] as List<CartItem>;

            // Nếu giỏ hàng không tồn tại hoặc không có mục nào trong giỏ hàng, tạo mới một danh sách trống
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
            }

            double totalPayment = 0;
            int soluong = 0;
            // Tạo một danh sách sản phẩm để hiển thị trong giỏ hàng (lấy thông tin sản phẩm từ các mục trong giỏ hàng)
            //var productsInCart = new List<SanPham>();
            foreach (var item in cartItems)
            {
                var product = productDao.getProductById(item.ProductId);
                if (product != null)
                {
                    //productsInCart.Add(product);
                    item.ProductId = product.MaSP;
                    item.image = product.HinhAnh;
                    item.name = product.TenSP;
                    item.mota = product.MoTaSP;
                    item.cost = product.GiaBanSP;
                    if(item.Price == 0) {
                        item.Price = product.GiaBanSP;
                    }
                    totalPayment += item.Price;
                    soluong += item.Quantity;
                    ViewBag.TotalPayment = totalPayment;
                }
            }
            Session["Soluong"] = cartItems.Count;
            var products = productDao.findAllSanPham();
            var productList = productDao.getProductSub(); 
            var viewModel = new ProductViewModel
            {
                Cart = cartItems,
                Products = products,
                ProductSub = productList

            };
            return View(viewModel);
        }


        [HttpGet]
        public ActionResult GetTotalCartJson()
        {
            var cart = Session["Cart"] as List<CartItem>;
            return Json(cart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApplyDiscount(string discount)
        {
            if (!string.IsNullOrEmpty(discount))
            {
                double discountInfo = discountDao.getDiscountByCode(discount); // Corrected method name to PascalCase
                if (discountInfo != 0)
                {
                    return Json(new { success = true, discountInfo });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid discount code." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Discount code is empty." });
            }
        }

        [HttpPost]
        public ActionResult UpdateQuantityAndPrice(string productId, int quantity, double price)
        {
            var cartItems = Session["Cart"] as List<CartItem>;

            // Find the item in the cart by productId
            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                // Update the quantity of the item
                cartItem.Quantity = quantity;
                cartItem.Price = price;
            }

            // Update the session with the modified cart
            Session["Cart"] = cartItems;

            // Return a success response
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult payment()
        {
            if (Session["Cart"] != null)
            {
                return RedirectToAction("Index", "Orders");
            }

            return View();
        }

        public ActionResult DeleteProductInCart(string productId)
        {
            var cartItems = Session["Cart"] as List<CartItem>;

            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                // Remove the item from the cart
                cartItems.Remove(cartItem);

                // Update the session
                Session["Cart"] = cartItems;
            }

            // Optionally, you can add a message to ViewBag or TempData indicating success or failure
            TempData["Message"] = "Xóa sản phẩm khỏi giỏ hàng thành công.";

            // Redirect to the cart view or wherever appropriate
            return RedirectToAction("Index");
        }

        public ActionResult getProductSub()
        {
            var productList = productDao.getProductSub(); // Truyền MaLH cần lấy vào hàm getProductSub()
            var model = new ProductViewModel
            {
                Products = productList
            };
            return View(model);
        }


    }
}