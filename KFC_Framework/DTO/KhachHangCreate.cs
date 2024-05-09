using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KFC_Framework.DTO
{
    public class KhachHangCreate
    {
        public string MaKH { get; set; }
        public string Email { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }

        public DateTime NgaySinh { get; set; }
        public string SoDT { get; set; }

        public string MatKhau { get; set; }
    }
}