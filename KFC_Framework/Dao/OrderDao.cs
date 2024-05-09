using Grpc.Core;
using KFC_Framework.DTO;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;
using System.Web.UI.WebControls;


namespace KFC_Framework.Dao
{
    public class OrderDao
    {
        public List<DonDatHang> getListOrders()
        {
            List<DonDatHang> orders;
            using(var db= new KFCEntities())
            {
                orders= db.DonDatHangs.ToList();
                return orders;
            }
        }

        public List<DonDatHang> getListOrdersByMakh(string makh)
        {
            List<DonDatHang> orders;
            using (var db = new KFCEntities())
            {
                orders = db.DonDatHangs.Where(x => x.MaKH == makh).ToList();
                return orders;
            }
        }

        public DonDatHang getByIdOrder(string id)
        {
            using (var db = new KFCEntities())
            {
                var idOrder = db.DonDatHangs.Where(x => x.MaDDH == id).FirstOrDefault();
                return idOrder;
            }
        }

        public List<CTDonDatHang> getDetailOrderById(string id)
        {
            using (var db = new KFCEntities())
            {
                List<CTDonDatHang> cTDonDatHangs = db.CTDonDatHangs.Where(x => x.MaDDH == id).ToList();
                return cTDonDatHangs;
            }
        }

        public DonDatHang delOrder(string id)
        {
            using (var db = new KFCEntities())
            {
                DonDatHang gg = db.DonDatHangs.Where(x => x.MaDDH == id).FirstOrDefault();
                if (gg != null)
                {
                    db.DonDatHangs.Remove(gg);
                    db.SaveChanges();
                }
                return gg;
            }
        }
        
        public DonDatHang editOrder(DonDatHang model)
        {
            using (var db = new KFCEntities())
            {
                var updateOrder = db.DonDatHangs.Find(model.MaDDH);
                if (updateOrder != null)
                {
                    updateOrder.HTThanhToan = model.HTThanhToan;
                    updateOrder.HTGiaoHang = model.HTGiaoHang;
                    updateOrder.NgayDH = model.NgayDH;
                    updateOrder.NgayGH = model.NgayGH;
                    updateOrder.TriGia = model.TriGia;
                    updateOrder.TinhTrang = model.TinhTrang;
                }
                db.SaveChanges();
            }
            return model;
        }

        public void InsertOrderInfo(string orderCode, OrdersForPay orderInfo, List<CartItem> cartItems, string makh)
        {
            using (KFCEntities context = new KFCEntities())
            {
                // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                try
                {
                    // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                DonDatHang newOrder = new DonDatHang
                {
                    MaDDH = orderCode, // Tạo mã đơn đặt hàng (bạn cần phải viết phương thức này)
                    MaKH = makh, // MaKH có thể được lấy từ orderInfo hoặc từ các nguồn dữ liệu khác
                    NgayDH = DateTime.Now,
                    NgayGH = DateTime.Now.AddDays(7), // Ví dụ, giao hàng sau 7 ngày
                    TenNguoiNhan = orderInfo.NameCustomer,
                    DiaChiNhan = orderInfo.NumHome + ", " + orderInfo.NameStreet + ", " + orderInfo.Country,
                    SDTNhanHang = orderInfo.Phone,
                    Email = orderInfo.Email,
                    HTThanhToan = orderInfo.OrderType, 
                    HTGiaoHang = "Giao hàng tận nơi",
                    TriGia = orderInfo.TotalPrice, 
                    LoaiKhachHang = makh== null ? "Khách hàng vãn lai" : "Khách hàng", 
                    TinhTrang = "Chờ xử lý" // Mặc định là chờ xử lý khi thêm đơn hàng mới
                };

                    // Thêm đơn đặt hàng mới vào cơ sở dữ liệu
                    context.DonDatHangs.Add(newOrder);
                    context.SaveChanges();
                    if (cartItems != null)
                    {
                        CreateCTDDH(newOrder.MaDDH, cartItems, newOrder);
                    }

                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    // Nếu bạn muốn hiển thị thông báo lỗi cho người dùng, bạn có thể trả về một Exception hoặc sử dụng một phương thức khác để xử lý lỗi
                    throw ex;
                }
            }
        }

        public void handleEmail(string orderCode,OrdersForPay orderInfo, List<CartItem> cartItems)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(orderInfo.Email); 
                mail.From = new MailAddress("2121005143@sv.ufm.edu.vn");
                mail.Subject = "Xác nhận đặt hàng thành công từ KFC Lê Văn Việt";

                //đính kèm hình ảnh vào email và ghi nội dung vào mail
                string imageUrl = "https://static.kfcvietnam.com.vn/images/content/home/carousel/lg/combo-dinner.webp?v=LKwy9L";
                string body = $"<br/><img style=\"width: 100%; height:100%;\" src='{imageUrl}' alt='Embedded Image' />";

                body +=
                    $"<br/>Mã Đơn hàng: <strong>{orderCode}</strong>" +
                    $"<br/>Họ Tên Khách hàng: <strong>{orderInfo.NameCustomer}</strong>" +
                    $"<br/>Email: <strong>{orderInfo.Email} </strong>" +
                    $"<br/>Số Điện Thoại: <strong>{orderInfo.Phone} </strong>" +
                    $"<br/>Địa chỉ:<strong>{orderInfo.NumHome}, {orderInfo.NameStreet}, {orderInfo.Country}</strong><br/>";

                foreach (var item in cartItems)
                {
                    string tenHang = item.name;
                    string soLuong = item.Quantity.ToString();
                    string thanhtien = String.Format("{0:0,000 VNĐ}", item.Price);
                    body +=
                        $"<br/>Tên Hàng:<strong>{tenHang} </strong>" +
                        $"<br/>Số Lượng:<strong>{soLuong}</strong>" +
                        $"<br/>Thành Tiền:<strong>{thanhtien}  </strong>" +
                        $"<br/>--------------------------------------";
                }
                body += $"<br/>Tổng Tiền:<strong>{String.Format("{0:0,000 VNĐ}", orderInfo.TotalPrice)}</strong><br/>";
                body += "Cảm ơn quý khách đã đặt hàng tại KFC.<br/>Chúc quý khách ngon miệng!<br/>KFC Lê Văn Việt cảm ơn quý khách!";

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("2121005143@sv.ufm.edu.vn", "votrongkha@13072003");
                client.Send(mail);
            }
            catch (SmtpFailedRecipientException ex)
            {
                //lblStatus.Text = "Mail không được gởi! " + ex.FailedRecipient;
            }
        }

        public void InsertBillInfo(DonDatHang donDatHang, List<CTDonDatHang> cTDonDatHangs, string manv)
        {
            using (KFCEntities context = new KFCEntities())
            {
                // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                try
                {
                    // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                    HoaDon newHoaDon = new HoaDon
                    {
                        MaHD = GenerateBillCode(), // Tạo mã đơn đặt hàng (bạn cần phải viết phương thức này)
                        MaDDH = donDatHang.MaDDH, // MaKH có thể được lấy từ orderInfo hoặc từ các nguồn dữ liệu khác
                        MaNV = manv,
                        NgayBan = donDatHang.NgayDH, // Ví dụ, giao hàng sau 7 ngày                  
                        TriGia = donDatHang.TriGia,
                    };

                    donDatHang.TinhTrang = "Hoàn thành";
                    context.DonDatHangs.AddOrUpdate(donDatHang);
                    // Thêm đơn đặt hàng mới vào cơ sở dữ liệu
                    context.HoaDons.Add(newHoaDon);
                    context.SaveChanges();
                    if (cTDonDatHangs != null)
                    {
                        CreateCTHD(newHoaDon.MaHD, cTDonDatHangs);
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    // Nếu bạn muốn hiển thị thông báo lỗi cho người dùng, bạn có thể trả về một Exception hoặc sử dụng một phương thức khác để xử lý lỗi
                    throw ex;
                }
            }
        }
        
        public string GenerateOrderCode()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public string GenerateBillCode()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        // Tạo chi tiết đơn đặt hàng
        public void CreateCTDDH(string MaDDH, List<CartItem> cartItems, DonDatHang donDatHang)
        {
            foreach(var item in cartItems)
            {
                using (KFCEntities context = new KFCEntities())
                {
                    // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                    try
                    {
                        // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                        CTDonDatHang newOrder = new CTDonDatHang
                        {
                            MaDDH = MaDDH, // Tạo mã đơn đặt hàng (bạn cần phải viết phương thức này)
                            MaSP = item.ProductId,
                            SoLuong = item.Quantity,
                            GiaBan = (int)item.cost,
                            TongTien = (int)item.Price //Có edit do lỗi
                        };

                        // Thêm đơn đặt hàng mới vào cơ sở dữ liệu
                        context.CTDonDatHangs.Add(newOrder);
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                        throw ex;
                    }
                }
            }
        }


        // Tạo chi tiết hóa đơn
        public void CreateCTHD(string MaHD, List<CTDonDatHang> cTDonDatHangs)
        {
            foreach (var item in cTDonDatHangs)
            {
                using (KFCEntities context = new KFCEntities())
                {
                    // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                    try
                    {
                        // Tạo một đối tượng DonDatHang mới từ dữ liệu trong orderInfo
                        CTHoaDon newOrder = new CTHoaDon
                        {
                            MaHD = MaHD, // Tạo mã đơn đặt hàng (bạn cần phải viết phương thức này)
                            MaSP = item.MaSP,
                            SoLuong = item.SoLuong,
                            GiaBan = (int)item.GiaBan,
                            TongTien = (int)item.TongTien //Có edit do lỗi
                        };

                        // Thêm đơn đặt hàng mới vào cơ sở dữ liệu
                        context.CTHoaDons.Add(newOrder);
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                        throw ex;
                    }
                }
            }
        }
        
        public void delOrder_DetailOrder(string maddh)
        {
            using (var db = new KFCEntities())
            {
                var chiTietDonDatHangs = db.CTDonDatHangs.Where(c => c.MaDDH == maddh).ToList();
                if (chiTietDonDatHangs.Any())
                {
                    db.CTDonDatHangs.RemoveRange(chiTietDonDatHangs);
                    db.SaveChanges();
                }

                // Tìm và xóa dữ liệu từ bảng DonDatHang dựa vào MaDDH
                var donDatHang = db.DonDatHangs.FirstOrDefault(d => d.MaDDH == maddh);
                donDatHang.TinhTrang = "Đã hủy";
                if (donDatHang != null)
                {
                    db.DonDatHangs.AddOrUpdate(donDatHang);
                    //db.DonDatHangs.Remove(donDatHang);
                    db.SaveChanges();
                }
            }
        }
        


    }
}