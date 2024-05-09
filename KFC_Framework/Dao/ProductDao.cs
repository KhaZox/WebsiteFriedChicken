using KFC_Framework.Models;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KFC_Framework.Dao
{
    public class ProductDetailDTO
    {
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBanSP { get; set; }
    }
    public class ProductDao
    {
        public ProductDao() { }
        public List<SanPham> GetSanPhamListByMaLH(string maLH)
        {
            List<SanPham> sanPhamList;

            using (var db = new KFCEntities()) // Create an instance of your Entity Framework context
            {
                sanPhamList = db.SanPhams.Where(s => s.MaLH == maLH).ToList();
            }
            return sanPhamList;
        }

        public List<SanPham> findAllSanPham()
        {
            //List<SanPham> sanPhamList;

            using (var db = new KFCEntities()) // Create an instance of your Entity Framework context
            {
                // Retrieve data from your database based on the provided MaLH
                //sanPhamList = db.SanPhams.ToList();
                return db.SanPhams.Include("LoaiHang").Include("GiamGia").ToList();

            }

            //return sanPhamList;
        }

        public Dictionary<string, List<SanPham>> GroupProductsByCategory(List<SanPham> products)
        {
            return products.GroupBy(p => p.MaLH).ToDictionary(g => g.Key, g => g.ToList());
        }

        public SanPham getProductById(string id)
        {
            SanPham sanPham = null;
            using (var db = new KFCEntities())
            {
                sanPham = db.SanPhams.Where(p => p.MaSP == id).FirstOrDefault();
            }
            return sanPham;
        }

        public string GetTenLHByMaSPMaLH(string maSP, string maLH)
        {
            using (var db = new KFCEntities())
            {
                var sanPham = db.SanPhams.FirstOrDefault(sp => sp.MaSP == maSP && sp.MaLH == maLH);

                if (sanPham != null)
                {
                    return sanPham.LoaiHang.TenLH;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<LoaiHang> getTenLoaiHang()
        {
            List<LoaiHang> lh;
            using (var db = new KFCEntities())
            {
                lh = db.LoaiHangs.ToList();
            }
            return lh;
        }
        public List<GiamGia> getMaGiamGia()
        {
            List<GiamGia> mgg;
            using (var db = new KFCEntities())
            {
                mgg = db.GiamGias.ToList();
            }
            return mgg;
        }
        public List<SanPham> getProductSub()
        {
            List<SanPham> sp;
            using (var db = new KFCEntities())
            {
                sp = db.SanPhams.Where(x => x.MaLH == "LH008").ToList(); 
            }
            return sp;
        }




        #region Action Manager Product
        public void AddProduct(SanPham product)
        {
            using (var db = new KFCEntities())
            {
                var newSanPham = new SanPham
                {
                    MaLH = product.MaLH,
                    MaGG = product.MaGG,
                    MaSP = product.MaSP,
                    TenSP = product.TenSP,
                    GiaBanSP = product.GiaBanSP, // example value
                    MoTaSP = product.MoTaSP,
                    HinhAnh = product.HinhAnh
                };
                db.SanPhams.Add(product);
                db.SaveChanges();

            }
        }
        public SanPham deleteProduct(string id)
        {
            using (var db = new KFCEntities())
            {
                // Find the SanPham record
                SanPham deletesanPham = db.SanPhams.FirstOrDefault(item => item.MaSP == id);

                if (deletesanPham != null)
                {
                    // Find related ChiTietNVL records
                    var relatedChiTietNVLs = db.ChiTietNVLs.Where(item => item.MaSP == id).ToList();

                    // Remove related ChiTietNVL records
                    db.ChiTietNVLs.RemoveRange(relatedChiTietNVLs);

                    // Remove the SanPham record
                    db.SanPhams.Remove(deletesanPham);

                    // Save changes to the database
                    db.SaveChanges();
                }

                return deletesanPham;
            }
        }
        public int getQuanlityProduct(string id)
        {
            using (var db = new KFCEntities())
            {
                // Fetch the quantity data from the database
                int? quantity = db.ChiTietNVLs
                                   .Where(x => x.MaSP == id)
                                   .Select(s => (int?)s.SoLuong) // Cast to nullable int
                                   .FirstOrDefault();

                // Return the quantity value if not null, otherwise return 0
                return quantity ?? 0;
            }
        }

        //Edit Product
        public SanPham editProduct(SanPham model)
        {
            using (var db = new KFCEntities())
            {
                var data = db.SanPhams.FirstOrDefault(x => x.MaSP == model.MaSP);
                if (data != null)
                {
                    data.MaLH = model.MaLH;
                    data.MaGG = model.MaGG;
                    data.TenSP = model.TenSP;
                    data.GiaBanSP = model.GiaBanSP;
                    data.MoTaSP = model.MoTaSP;
                    data.HinhAnh = model.HinhAnh;
                    db.SaveChanges();
                }

                return data;
            }
        }
        #endregion

        #region Manager Warehouse
        public List<ChiTietKho> listWarehouse()
        {
            using (var db = new KFCEntities())
            {
                return db.ChiTietKhoes.Include("Kho").Include("NguyenVL").ToList();
            }
        }
        public List<Kho> getMaKho()
        {
            List<Kho> kho;
            using (var db = new KFCEntities())
            {
                kho = db.Khoes.ToList();
            }
            return kho;
        }
        public List<NguyenVL> getNguyenVLs()
        {
            List<NguyenVL> nvl;
            using (var db = new KFCEntities())
            {
                nvl = db.NguyenVLs.ToList();
            }
            return nvl;
        }
        public ChiTietKho addWarehouse(ChiTietKho model)
        {
            using(var db= new KFCEntities())
            {
                var add= db.ChiTietKhoes.Add(model);
                db.SaveChanges();
                return add;
            }
        }
        public ChiTietKho deleteWarehouse(string maKho, string maNVL)
        {
            using (var db = new KFCEntities())
            {
                var product = db.ChiTietKhoes.FirstOrDefault(p => p.MaKho == maKho && p.MaNVL == maNVL);

                db.ChiTietKhoes.Remove(product);
                db.SaveChanges();
                return product;
            }
        }
        public ChiTietKho editWarehouseGet(string maKho, string maNVL)
        {
            using (var db = new KFCEntities())
            {
                var wareHouse = db.ChiTietKhoes.FirstOrDefault(p => p.MaKho == maKho && p.MaNVL == maNVL);
                return wareHouse;
            }
        }
        public ChiTietKho editWarehousePost(ChiTietKho wareHouse)
        {
            using (var db = new KFCEntities())
            {
                db.Entry(wareHouse).State = EntityState.Modified;
                db.SaveChanges();
                return wareHouse;
            }
        }

        #endregion
    }


}