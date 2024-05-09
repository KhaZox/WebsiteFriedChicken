using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace KFC_Framework.Dao
{
    public class DiscountDao
    {
        public DiscountDao() { }

        public double getDiscountByCode(string code)
        {
            using (var dbContext = new KFCEntities())
            {
                GiamGia discount = dbContext.GiamGias.FirstOrDefault(u => u.MaGG == code);
                if (discount != null)
                {
                    return discount.PhanTram;
                }
                else
                {
                    return 0;
                }
            }
        }
        public List<GiamGia> getListDiscount()
        {
            List<GiamGia> gg;
            using (var db = new KFCEntities())
            {
                gg = db.GiamGias.ToList();
            }
            return gg;
        }
        public GiamGia getByIdDiscount(string id)
        {
            using (var db = new KFCEntities())
            {
                var idDiscount = db.GiamGias.Where(x => x.MaGG == id).FirstOrDefault();
                return idDiscount;
            }
        }
        public void addDiscount(GiamGia model)
        {
            using (var db = new KFCEntities())
            {
                var newDiscount = new GiamGia
                {
                    MaGG = DiscountCode(),
                    TenSuKien = model.TenSuKien,
                    PhanTram = model.PhanTram,
                    ThongTinSuKien = model.ThongTinSuKien,
                    ThoiGianBD = model.ThoiGianBD,
                    ThoiGianKT = model.ThoiGianKT
                };
                db.GiamGias.Add(newDiscount);
                db.SaveChanges();
            }
        }
        private string DiscountCode()
        {
            string code;
            using (var db = new KFCEntities())
            {
                do
                {
                    // Tạo mã ngẫu nhiên
                    var random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    code = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

                    // Kiểm tra xem mã có tồn tại trong cơ sở dữ liệu chưa
                } while (db.GiamGias.Any(gg => gg.MaGG == code));
            }
            return code;
        }

        public void delDiscount(string id)
        {
            using (var db = new KFCEntities())
            {
                db.Database.ExecuteSqlCommand("EXEC DeleteGiamGiaAndUpdateSanPham @MaGG", new SqlParameter("@MaGG", id));
                db.SaveChanges();
            }
        }

        public GiamGia editDiscount(GiamGia model)
        {
            using(var db= new KFCEntities())
            {
                var updateDiscount = db.GiamGias.Find(model.MaGG);
                if (updateDiscount != null)
                {
                    updateDiscount.TenSuKien = model.TenSuKien;
                    updateDiscount.PhanTram = model.PhanTram;
                    updateDiscount.ThongTinSuKien = model.ThongTinSuKien;
                    updateDiscount.ThoiGianBD = model.ThoiGianBD;
                    updateDiscount.ThoiGianKT = model.ThoiGianKT;
                }
                db.SaveChanges();
            }
            return model;
        }
    }
}