using KFC_Framework.Dao;
using KFC_Framework.DTO;

using KFC_Framework.Models;
using KFC_Framework.ViewModels;
using MoMo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Controllers
{
    public class OrdersController : Controller
    {
        private OrderDao orderDao = new OrderDao();

        private CustomerDao customerDao = new CustomerDao();

        // GET: Orders
        public ActionResult Index()
        {
            var cartItems = new List<CartItem>();
            ViewBag.TotalPayment = Session["TotalPayment"];
            string id = Session["MaTK"] as string;
            KhachHang khachHang = customerDao.getByID(id);
            var orderViewModel = new OrderViewModel
            {
                Cart = cartItems,
                khachHang = khachHang
            };
            return View(orderViewModel);
        }

        //POST: Payment
        [HttpPost]
        public ActionResult PayOrders(OrdersForPay ordersForPay)
        {
            if (ordersForPay != null)
            {
                string matk = Session["MaTK"] as string;
                string khachHang = null;
                if (matk != null)
                {
                    khachHang = customerDao.getByID(matk).MaKH;
                }
                var cartItems = Session["Cart"] as List<CartItem>;
                string orderCode = orderDao.GenerateOrderCode();

                orderDao.InsertOrderInfo(orderCode, ordersForPay, cartItems, khachHang);
                orderDao.handleEmail(orderCode, ordersForPay, cartItems);
                Session.Remove("Cart");
                return Json(new { success = true, orderCode = orderCode, message = "Đơn hàng đã được xử lý thành công!" }); 
            }
            else
            {
                return Json(new { success = false, message = "Không có dữ liệu đơn hàng để xử lý!" });
            }
        }

        public ActionResult Payment(OrdersForPay ordersForPay)
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng KFC";
            string returnUrl = "https://localhost:44358/Orders/ConfirmPaymentClient";
            string notifyurl = "https://localhost:44358/Orders/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = ordersForPay.TotalPrice.ToString();
            string orderid = orderDao.GenerateOrderCode();
            string requestId = orderDao.GenerateOrderCode();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);
            var link = jmessage.GetValue("payUrl").ToString();

            TempData["OrdersForPay"] = ordersForPay;

            return Json(new { success = true, link = link });

            //return Redirect(jmessage.GetValue("payUrl").ToString());
        }


        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient(Result result)
        {
            string matk = Session["MaTK"] as string;
            string khachHang = null;
            if (matk != null)
            {
                khachHang = customerDao.getByID(matk).MaKH;
            }
            var cartItems = Session["Cart"] as List<CartItem>;

            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            OrdersForPay ordersForPay = TempData["OrdersForPay"] as OrdersForPay;
            if (rErrorCode == "0")
            {
                orderDao.InsertOrderInfo(rOrderId, ordersForPay, cartItems, khachHang);
            }
            return View();
        }

        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
            String a = "";
        }

        [HttpPost]
        public ActionResult SaveTotalPayment(string totalPayment)
        {
            Session["TotalPayment"] = totalPayment; // Lưu giá trị totalPayment vào Session
            return RedirectToAction("Index", "Orders"); // Chuyển hướng đến action Index của controller Orders
        }
    }
}
