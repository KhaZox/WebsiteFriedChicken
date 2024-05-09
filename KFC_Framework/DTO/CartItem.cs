using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KFC_Framework.DTO
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double cost { get; set; }
        public double Price { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string mota { get; set; }
    }
}