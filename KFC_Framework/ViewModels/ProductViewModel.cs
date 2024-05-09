using KFC_Framework.DTO;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KFC_Framework.ViewModels
{
    public class ProductViewModel
    {
        public List<CartItem> Cart { get; set; }
        public List<SanPham> Products { get; set; }
        public List<SanPham> ProductSub { get; set; }
        public List<LoaiHang> TypeProducts { get; set; }
        public List<GiamGia> Discount { get; set; }
        public List<ChiTietNVL> detailMaterial { get; set; }
        public SanPham SanPham { get; set; }
    }
}