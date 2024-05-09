using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KFC_Framework.DTO
{
    public class OrdersForPay
    {
        public string MaKH { get; set; }
        public string NumHome { get; set; }
        public string NameStreet { get; set; }
        public string Country { get; set; }
        public string NoteOrder { get; set; }
        public string LastName { get; set; }
        public string NameCustomer { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string OrderType { get; set; }
        public int TotalPrice { get; set; } 
    }
}