using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KFC_Framework.Dao
{
    public class EmployeeDao
    {
        public EmployeeDao() { }

        public NhanVien getByMaTK(string id)
        {
            using (var db = new KFCEntities())
            {
                NhanVien nhanVien = db.NhanViens.Where(x => x.MaTK == id).FirstOrDefault();
                return nhanVien;
            }
        }
        public NhanVien updateProfile(NhanVien profile, string matk)
        {
            using(var db = new KFCEntities())
            {
                NhanVien existingProfile = db.NhanViens.FirstOrDefault(k => k.MaTK == matk);
                if (existingProfile != null)
                {
                    // Update the profile properties
                    existingProfile.HoTen = profile.HoTen;
                    existingProfile.NgaySinh = profile.NgaySinh;
                    existingProfile.GioiTinh = profile.GioiTinh;
                    existingProfile.DiaChi = profile.DiaChi;
                    existingProfile.Email = profile.Email;
                    existingProfile.SoDT = profile.SoDT;

                    // Save changes to the database
                    db.SaveChanges();
                }
                return existingProfile;
            }
        }
    }
}