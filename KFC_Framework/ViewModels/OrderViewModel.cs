using KFC_Framework.DTO;
using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KFC_Framework.ViewModels
{
    public class OrderViewModel
    {
        public List<CartItem> Cart { get; set; }
        public KhachHang khachHang { get; set; }
        public List<DonDatHang> donDatHang { get; set;}
    }
}