using KFC_Framework.DTO;
using KFC_Framework.Models;
using KFC_Framework.Orther;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;

namespace KFC_Framework.Dao
{
    public class AccountDao
    {

        public AccountDao() { }
        
        public bool IsValidUser(string username, string password)
        {
            // Assuming you are using Entity Framework for data access
            using (var dbContext = new KFCEntities())
            {
                // Check if user exists with the provided username and hashed password
                var user = dbContext.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == username && u.MatKhau == password);
                return user != null;
            }
        }

        public List<TaiKhoan> getAllAccount()
        {
            List<TaiKhoan> list;
            using(var db =  new KFCEntities())
            {
                list= db.TaiKhoans.ToList();
                return list;
            }
        }
       
        public TaiKhoan getByIdAccount(string id)
        {
            using(var db= new KFCEntities())
            {
                var idAccount=db.TaiKhoans.Where(x => x.MaTK == id).FirstOrDefault();
                return idAccount;
            }
        }

        public void delAccountCustomer(string id)
        { 
            using (var db = new KFCEntities())
            {
                db.Database.ExecuteSqlCommand("EXEC DeleteCustomerAccount @MaTK", new SqlParameter("@MaTK", id));
                db.SaveChanges();
            }
        }

        public TaiKhoan delAccount(string id)
        {
            using (var db = new KFCEntities())
            {
                TaiKhoan account = db.TaiKhoans.Where(x => x.MaTK == id).FirstOrDefault();
                if (account.LoaiTK != "Khách hàng")
                {
                    NhanVien nv = db.NhanViens.Where(x => x.MaTK == id).FirstOrDefault();
                    db.NhanViens.Remove(nv);
                }
                else
                {
                    KhachHang khachHang = db.KhachHangs.Where(x => x.MaTK == id).FirstOrDefault();
                    db.KhachHangs.Remove(khachHang);
                }

                if (account != null)
                {
                    db.TaiKhoans.Remove(account);
                    db.SaveChanges();
                }
                return account;
            }
        }

        public TaiKhoan editAccount(TaiKhoan model)
        {
            using (var db = new KFCEntities())
            {
                var data = db.TaiKhoans.FirstOrDefault(x => x.MaTK == model.MaTK);
                if (data != null)
                {
                    data.TenDangNhap = model.TenDangNhap;
                    data.MatKhau = model.MatKhau;
                    data.LoaiTK = model.LoaiTK;
                    db.SaveChanges();
                }
                return data;
            }
        }

        public void addAccount(TaiKhoan model)
        {
            using (var db = new KFCEntities())
            {
                var matk = GenerateCode("TK");
                var newAccount = new TaiKhoan
                {
                    MaTK = matk,
                    TenDangNhap = model.TenDangNhap,
                    MatKhau = model.MatKhau,
                    LoaiTK = model.LoaiTK,
                    NgayTao = model.NgayTao
                };
                db.TaiKhoans.Add(newAccount);
                db.SaveChanges();
                if (model.LoaiTK== "Khách hàng")
                {
                    createCustomer(matk);
                }
                else
                {
                    createStaff(matk, model.LoaiTK);
                }
            }
        }

        public void createStaff(string matk, string chucvu)
        {
            using(var db = new KFCEntities())
            {
                var newStaff = new NhanVien
                {
                    MaNV = GenerateCode("NV"),
                    MaTK = matk,
                    HoTen = " ",
                    DiaChi = " ",
                    Email = " ",
                    SoDT = " ",
                    HinhAnh = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fthespiritofsaigon.net%2Favatar-vo-danh%2F&psig=AOvVaw0Br-lCX6BeoNdvyicCXOw_&ust=1714205439888000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCPjo-YS334UDFQAAAAAdAAAAABAE",
                    ChucVu = chucvu
                };
                db.NhanViens.Add(newStaff);
                db.SaveChanges();
            }
        }
       
        public void createCustomer(string matk)
        {
            using (var db = new KFCEntities())
            {
                var newCustomer = new KhachHang
                {
                    MaKH = GenerateCode("KH"),
                    MaTK = matk,
                    HoTen = " ",
                    DiaChi = " ",
                    Email = " ",
                    SoDT = " "
                };
                db.KhachHangs.Add(newCustomer);
                db.SaveChanges();
            }
        }

        public string GenerateCode(string type)
        {
            int counter = 0;
            string prefix = type;

            // Khởi tạo đối tượng context
            using (var db = new KFCEntities())
            {
                // Lấy mã lớn nhất từ cơ sở dữ liệu
                string maxCode = "";
                if (type == "NV")
                    maxCode = db.NhanViens.Max(nv => nv.MaNV);
                else if (type == "KH")
                    maxCode = db.KhachHangs.Max(kh => kh.MaKH);
                else if (type == "TK")
                    maxCode = db.TaiKhoans.Max(tk => tk.MaTK);

                // Kiểm tra xem mã đã tồn tại chưa
                if (!string.IsNullOrEmpty(maxCode))
                {
                    // Trích xuất số cuối cùng của mã lớn nhất để cập nhật counter
                    int lastNumber = int.Parse(maxCode.Replace(prefix, ""));
                    counter = lastNumber + 1;
                }

                // Tạo mã mới
                string number = counter.ToString().PadLeft(3, '0');
                string newCode = prefix + number;

                // Tăng counter lên để chuẩn bị cho mã tiếp theo
                counter++;

                return newCode;
            }
        }

        public string Role(string username)
        {
            // Assuming you are using Entity Framework for data access
            using (var dbContext = new KFCEntities())
            {
                // Check if user exists with the provided username and hashed password
                var user = dbContext.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == username);
                return user.LoaiTK;
            }
        }

        public KhachHang getKhachHangById(string matk)
        {
            KhachHang khachHang = new KhachHang();
            using (var db = new KFCEntities()) // Create an instance of your Entity Framework context
            {
                // Retrieve data from your database based on the provided MaLH
                //khachHang = (from kh in db.KhachHangs
                //                     join tk in db.TaiKhoans on kh.MaTK equals tk.MaTK
                //                     where tk.TenDangNhap == username
                //                     select kh).FirstOrDefault();
                khachHang = (from kh in db.KhachHangs
                             join tk in db.TaiKhoans on kh.MaTK equals tk.MaTK
                             where tk.MaTK == matk
                             select kh).FirstOrDefault();

            }

            return khachHang;
        }

        public NhanVien getNhanVienById(string username)
        {
            NhanVien nhanvien = new NhanVien();
            using (var db = new KFCEntities()) // Create an instance of your Entity Framework context
            {
                // Retrieve data from your database based on the provided MaLH
                nhanvien = (from kh in db.NhanViens
                             join tk in db.TaiKhoans on kh.MaTK equals tk.MaTK
                             where tk.TenDangNhap == username
                             select kh).FirstOrDefault();

            }

            return nhanvien;
        }

        public NhanVien updateInforStaff(NhanVien model)
        {
            using(var db = new KFCEntities())
            {
                var data = db.NhanViens.Find(model.MaNV);
                if (data != null)
                {
                    data.HoTen = model.HoTen;
                    data.NgaySinh = model.NgaySinh;
                    data.GioiTinh = model.GioiTinh;
                    data.DiaChi = model.DiaChi; 
                    data.Email=model.Email;
                    data.SoDT= model.SoDT;
                }
                db.SaveChanges();
            }
            return model;
        }

        public string getMaTK()
        {
            using (var dbContext = new KFCEntities())
            {
                // Lấy mã khách hàng lớn nhất
                var maxTKID = dbContext.TaiKhoans
                                          .OrderByDescending(kh => kh.MaTK)
                                          .Select(kh => kh.MaTK)
                                          .FirstOrDefault();
                return maxTKID;
            }
        }
        
        public string analystCode(string code)
        {
            // Kiểm tra chuỗi code có hợp lệ hay không
            if (!string.IsNullOrEmpty(code) && code.Length > 2)
            {
                // Giả sử mã luôn bắt đầu bằng 2 ký tự chữ và theo sau là các số
                string prefix = code.Substring(0, 2);  // Lấy 2 ký tự đầu tiên
                string numberPart = code.Substring(2); // Lấy phần còn lại là số

                // Chuyển phần số thành int và tăng lên 1
                int num = int.Parse(numberPart) + 1;

                // Tạo chuỗi kết quả, sử dụng "D3" để đảm bảo có ít nhất 3 chữ số
                string result = prefix + num.ToString("D3");

                return result;
            }
            // Trả về null nếu input không hợp lệ
            return null;
        }

        public TaiKhoan getAccountById(string username)
        {

            using (var dbContext = new KFCEntities())
            {
                // Check if user exists with the provided username and hashed password
                var user = dbContext.TaiKhoans.FirstOrDefault(u => u.MaTK == username);
                return user;
            }
        }
       
        public string passwordRandom(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";

            // Khởi tạo một đối tượng Random
            Random random = new Random();

            // Sử dụng StringBuilder để xây dựng mật khẩu ngẫu nhiên
            StringBuilder password = new StringBuilder(length);

            // Lặp để chọn ngẫu nhiên các ký tự từ validChars
            for (int i = 0; i < length; i++)
            {
                // Chọn một ký tự ngẫu nhiên từ validChars
                char randomChar = validChars[random.Next(validChars.Length)];

                // Thêm ký tự ngẫu nhiên vào mật khẩu
                password.Append(randomChar);
            }
            return password.ToString();
        }
        
        public void handleEmail(string email, string password)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("2121005143@sv.ufm.edu.vn");
                mail.Subject = "[KFC] Yêu cầu mật khẩu mới";

                //đính kèm hình ảnh vào email và ghi nội dung vào mail
                string imageUrl = "https://i.pinimg.com/564x/58/c4/7e/58c47e39f57b8ecd9fac6f541741a128.jpg";
                string body = $"<br/><img style=\"width: 100%; height:300px;\" src='{imageUrl}' alt='Security_Img' />";
                body +=
                    $"<br/>Bạn vừa yêu cầu lấy lại mật khẩu trên <strong>[KFC]</strong>" +
                    $"<br/>Dưới đây là mật khẩu mới của bạn:" +
                    $"<br/><strong>Email</strong>: {email}" +
                    $"<br/><strong>Mật khẩu</strong>: {password}" +
                    $"<br/>Hãy đổi mật khẩu sau khi đăng nhập thành công.<br/>" +
                    $"<br/>Chân thành cảm ơn bạn đã tin tưởng và sử dụng KFC. Chúc bạn hưởng thức món ăn vui vẻ!" +
                    $"<br/>Đây là email trả lời tự động vui lòng không trả lời.";

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

            }
        }
        
        public void updatPassword(string email, string password)
        {
            using (var db = new KFCEntities())
            {
                var data = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == email);
                if (data != null)
                {
                    data.MatKhau = password;
                    db.SaveChanges();
                }
            }
        }
        
        public void loginWithGoogle(GoogleProfile profile)
        {
            using (var db = new KFCEntities())
            {
                if (profile != null && profile.email != null)
                {
                    TaiKhoan taiKhoan = new TaiKhoan()
                    {
                        MaTK = profile.id,
                        TenDangNhap = profile.email,
                        MatKhau = "LoginWithGooogle",
                        LoaiTK = "Khách hàng"
                    };
                    db.TaiKhoans.Add(taiKhoan);

                    KhachHang khachHang = new KhachHang()
                    {
                        MaKH = GenerateCode("KH"),
                        MaTK = profile.id,
                        HoTen = profile.family_name + " " + profile.given_name,
                        Email = profile.email,
                        SoDT = " ",
                        DiaChi = " "
                    };
                    db.KhachHangs.Add(khachHang);

                    db.SaveChanges();
                }
                else
                {
                    // Handle the case where profile or profile.Emails is null
                }
            }
        }

        public void loginWithFacebook(LoginFacebook profile)
        {
            using (var db = new KFCEntities())
            {
                if (profile != null)
                {
                    TaiKhoan taiKhoan = new TaiKhoan()
                    {
                        MaTK = profile.id,
                        TenDangNhap = "LoginFacebook",
                        MatKhau = "LoginWithFacebook",
                        LoaiTK = "Khách hàng",
                        NgayTao= DateTime.Now,
                    };
                    db.TaiKhoans.Add(taiKhoan);

                    KhachHang khachHang = new KhachHang()
                    {
                        MaKH = GenerateCode("KH"),
                        MaTK = profile.id,
                        HoTen = profile.first_name + " " + profile.last_name,
                        Email = " ",
                        SoDT = " ",
                        DiaChi = " "
                    };
                    db.KhachHangs.Add(khachHang);

                    db.SaveChanges();
                }
                else
                {
                    // Handle the case where profile or profile.Emails is null
                }
            }
        }
    }

}