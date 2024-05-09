﻿using KFC_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KFC_Framework.DTO;
using System.Data.Entity.Validation;

namespace KFC_Framework.Dao
{
    public class CustomerDao
    {
        private KFCEntities db = new KFCEntities();
        private AccountDao accountDao = new AccountDao();
        
        public void Create(KhachHangCreate khachHangCreate)
        {
            using (var context = new KFCEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        string Matk = accountDao.getMaTK();
                        string codes = accountDao.analystCode(Matk);
                        string MaKH = getMaKH();
                        string codeMaKh = accountDao.analystCode(MaKH);
                        // Create a new TaiKhoan (account)
                        var newTaiKhoan = new TaiKhoan()
                        {
                            MaTK = codes, // Assuming MaTK is auto-generated by the database
                            TenDangNhap = khachHangCreate.Email,
                            MatKhau = khachHangCreate.MatKhau,
                            LoaiTK = "Khách hàng" // No need for 'as string'
                        };
                        context.TaiKhoans.Add(newTaiKhoan);
                        context.SaveChanges(); // Save changes to generate MaTK (if it's auto-generated)

                        // Create a new KhachHang (customer)
                        var newKhachHang = new KhachHang()
                        {
                            MaKH = codeMaKh, // Assuming MaKH is auto-generated by the database
                            MaTK = codes,
                            HoTen = khachHangCreate.Ho + " " + khachHangCreate.Ten, // Concatenate Ho and Ten
                            DiaChi = "abc" as string,
                            Email = khachHangCreate.Email,
                            SoDT = khachHangCreate.SoDT
                        };
                        context.KhachHangs.Add(newKhachHang);
                        context.SaveChanges(); // Save changes to generate MaKH (if it's auto-generated)
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();
                        // Handle or log the exception
                        throw new Exception("An error occurred while updating the entries. See the inner exception for detail.", ex);
                    }
                }
            }
        }

        public string getMaKH()
        {
            using (var dbContext = new KFCEntities())
            {
                // Lấy mã khách hàng lớn nhất
                var maxCustomerID = dbContext.KhachHangs
                                          .OrderByDescending(kh => kh.MaKH)
                                          .Select(kh => kh.MaKH)
                                          .FirstOrDefault();
                return maxCustomerID;
            }
        }

        public TaiKhoan getByMaTK(string id)
        {
            using (var db = new KFCEntities())
            {
                TaiKhoan taiKhoan = db.TaiKhoans.Where(x => x.TenDangNhap == id).FirstOrDefault();
                return taiKhoan;
            }
        }
       
        public KhachHang getByID(string id)
        {
            using (var db = new KFCEntities())
            {
                KhachHang khachHang = db.KhachHangs.Where(x => x.MaTK == id).FirstOrDefault();
                return khachHang;
            }
        }

        public string getPictureStaff(string email)
        {
            using (var db = new KFCEntities())
            {
                var matk= db.TaiKhoans.Where(x=> x.TenDangNhap.Equals(email)).Select(x=> x.MaTK).FirstOrDefault();
                var nhanvien = db.NhanViens.Where(x => x.MaTK == matk).FirstOrDefault();
                var picture = nhanvien.HinhAnh;
                return picture;
            }
        }


        public KhachHang updateProfile(KhachHang profile, string matk)
        {
            KhachHang existingProfile = db.KhachHangs.FirstOrDefault(k => k.MaTK == matk);

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