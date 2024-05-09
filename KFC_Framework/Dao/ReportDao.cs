using KFC_Framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using static KFC_Framework.Areas.admin.Controllers.managerReportController;

namespace KFC_Framework.Dao
{
    public class ReportDao
    {
        public ReportDao() { }

        public List<HoaDon> findAllBill()
        {
            List<HoaDon> hd;
            using (var db = new KFCEntities())
            {
                hd = db.HoaDons.ToList();
                return hd;
            }
        }
        public List<HoaDon> getMonthYear(int month, int year)
        {
            using (var db = new KFCEntities())
            {
                var salesData = db.HoaDons
                    .Where(h => h.NgayBan.Month == month && h.NgayBan.Year == year)
                    .GroupBy(h => new { h.NgayBan.Month, h.NgayBan.Year })
                    .Select(g => new HoaDon
                    {
                        NgayBan = new DateTime(g.Key.Year, g.Key.Month, 1),
                        TriGia = g.Sum(h => h.TriGia)
                    })
                    .ToList();

                return salesData;
            }
        }

        public string getOrderChart()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            using (var db = new KFCEntities())
            {
                int totalOrders = db.DonDatHangs.Count();
                // Thực hiện truy vấn để lấy dữ liệu từ bảng đơn hàng
                var query = db.DonDatHangs
                                  .GroupBy(o => o.TinhTrang)
                                  .Select(g => new { Status = g.Key, Count = g.Count() })
                                  .ToList();
                double totalPercentage = 0;
                foreach (var result in query)
                {
                    double percentage = (double)result.Count / totalOrders * 100;
                    // Đảm bảo giá trị phần trăm không vượt quá 100%
                    //percentage = Math.Min(percentage, 100);
                    //dataPoints.Add(new DataPoint(result.Status, percentage));
                    percentage = Math.Min(percentage, 100 - totalPercentage);
                    percentage = Math.Round(percentage, 2);
                    dataPoints.Add(new DataPoint(result.Status, percentage));
                    totalPercentage += percentage;
                }
            }

            // Chuyển đổi danh sách điểm dữ liệu thành chuỗi JSON
            return JsonConvert.SerializeObject(dataPoints);
        }
        [DataContract]
        public class DataPoint
        {
            public DataPoint(string label, double y)
            {
                this.Label = label;
                this.Y = y;
            }

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "label")]
            public string Label = "";

            //Explicitly setting the name to be used while serializing to JSON.
            [DataMember(Name = "y")]
            public Nullable<double> Y = null;
        }
    }
}