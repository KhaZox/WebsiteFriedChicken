using KFC_Framework.Dao;
using KFC_Framework.DTO;
using KFC_Framework.Models;
using KFC_Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;
using System.Configuration;
using ASPSnippets.GoogleAPI;
using System.Web.Script.Serialization;
using System.Net;
using System.Web.Helpers;
using KFC_Framework.Orther;
using OfficeOpenXml;

namespace KFC_Framework.Controllers
{
    public class AccountController : Controller
    {
        private AccountDao accountDao = new AccountDao();
        private CustomerDao customerDao = new CustomerDao();
        private OrderDao orderDao = new OrderDao();
        private KFCEntities db = new KFCEntities();


        # region Part Customer
        // GET: /Index
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // Authentication successful, redirect to some other action or page
                if (accountDao.IsValidUser(email, password))
                {
                    Session["LoggedInUser"] = email;
                    FormsAuthentication.SetAuthCookie(email, false);
                    Session["MaTK"] = customerDao.getByMaTK(email).MaTK;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Authentication failed, return back to the login page with an error message
                    ViewBag.ErrorMessage = "Invalid email or password";
                    return View("Index");
                }
            }
            else
            {
                // Authentication failed, return back to the login page with an error message
                ViewBag.ErrorMessage = "Email and password are required";
                return View("Index");
            }
        }

        // POST: /Logout
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["MaTK"] = null;
            Session["LoggedInUser"] = null;
            return RedirectToAction("Index", "Home");
        }

        // POST: /Register
        [HttpPost]
        public ActionResult Register(KhachHangCreate khachHangCreate)
        {
            if (khachHangCreate != null)
            {
                customerDao.Create(khachHangCreate);
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        // Get: /PersonAccount
        [HttpGet]
        public ActionResult PersonAccount()
        {
            return View();
        }

        // POST: /ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest()) // Check if the request is an AJAX call
                    return Json(new { success = false, message = "Validation failed." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            string matk = Session["MaTK"] as string;
            var user = accountDao.getAccountById(matk);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "User not found." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            // Verify current password
            if (user.MatKhau != model.CurrentPassword) // Ideally, use hashing comparison here
            {
                ModelState.AddModelError("", "The current password is incorrect.");
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "The current password is incorrect." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            // Check new password confirmation
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The new password and confirmation password do not match.");
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Passwords do not match." }, JsonRequestBehavior.AllowGet);

                return View(model);
            }

            // Update the password
            user.MatKhau = model.NewPassword; // You should hash this password
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Password successfully changed." }, JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");

            //return RedirectToAction("Index"); // Redirect to a relevant page
        }

        // POST: /UpdateProfile
        [HttpPost]
        public JsonResult UpdateProfile(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                //string email = Session["LoggedInUser"] as string;
                //khachHang = accountDao.getKhachHangById(email);

                string matk = Session["MaTK"] as string;
                customerDao.updateProfile(khachHang, matk);
                return Json(new { success = true, message = "Cập nhật thông tin thành công." });
            }
            else
            {
                return Json(new { success = false, message = "Update fail." });
            }
        }

        // POST: /delAccount
        [HttpPost]
        public ActionResult DeleteAccount(string id)
        {
            var delAccount = accountDao.getByIdAccount(id);
            if (delAccount != null)
            {
                accountDao.delAccountCustomer(id);
                Session["MaTK"] = null;
                Session["LoggedInUser"] = null;
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: /forgotPassword
        [HttpPost]
        public ActionResult forgotPassword(string email)
        {
            try
            {
                var password = accountDao.passwordRandom(10);
                accountDao.updatPassword(email, password);
                accountDao.handleEmail(email, password);
                return Json(new { success = true, message = "Email đặt lại mật khẩu đã được gửi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi gửi email đặt lại mật khẩu." });
            }
        }

        #endregion

        #region Part Login Social

        #region Login Facebook

        private Uri RediredtUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        [AllowAnonymous]
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["AppId"],
                client_secret = ConfigurationManager.AppSettings["AppSecret"],
                redirect_uri = RediredtUri.AbsoluteUri, // Sử dụng AbsolutePath thay vì AbsoluteUri
                respone_type = "code",
                scope = "public_profile",
            }); 
            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallBack(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["AppId"],
                client_secret = ConfigurationManager.AppSettings["AppSecret"],
                redirect_uri = RediredtUri.AbsoluteUri, // Sử dụng AbsolutePath thay vì AbsoluteUri
                code = code,
            });

            var accessToken = result.access_token;
            Session["AccessToken"] = accessToken;

            if (!string.IsNullOrEmpty(accessToken))
            {
                //Picture không được
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range,id");
                var loginFacebook = new LoginFacebook
                {
                    id = me.id,
                    email = me.email,
                    first_name = me.first_name,
                    last_name = me.last_name,
                    //picture = me.picture?.data?.url.ToString()
                };

                if (db.TaiKhoans.Find(loginFacebook.id) != null)
                {
                    Session["LoggedInUser"] = loginFacebook.first_name;
                    Session["MaTK"] = loginFacebook.id;
                    return RedirectToAction("Index", "Home");
                }

                accountDao.loginWithFacebook(loginFacebook);
                Session["LoggedInUser"] = loginFacebook.first_name;
                Session["MaTK"] = loginFacebook.id;
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion



        #region Completive Login Google

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void LoginWithGooglePlus()
        {
            GoogleConnect.ClientId = ConfigurationManager.AppSettings["AppGoogleId"];
            GoogleConnect.ClientSecret = ConfigurationManager.AppSettings["AppGoogleSecret"];
            GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];
            GoogleConnect.Authorize("profile", "email");
        }

        [ActionName("LoginWithGooglePlus")]
        public ActionResult LoginWithGooglePlusConfirmed()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                string code = Request.QueryString["code"];
                GoogleConnect googleConnector = new GoogleConnect();
                string json = googleConnector.Fetch("me", code);
                GoogleProfile profile = new JavaScriptSerializer().Deserialize<GoogleProfile>(json);

                if (db.TaiKhoans.Find(profile.id) != null)
                {
                    Session["MaTK"] = customerDao.getByMaTK(profile.email).MaTK;
                    Session["LoggedInUser"] = profile.given_name;
                    return RedirectToAction("Index", "Home");
                }

                accountDao.loginWithGoogle(profile);
                Session["LoggedInUser"] = profile.given_name;
                Session["MaTK"] = customerDao.getByMaTK(profile.email).MaTK;
            }

            if (Request.QueryString["error"] == "access_denied")
            {
                return Content("access_denied");
            }
            
            return RedirectToAction("Index", "Home");

        }

        #endregion


        #endregion

        #region Part Employee

        // GET: /LoginForEmployee
        [HttpGet]
        public ActionResult LoginForEmployee()
        {
            return View();
        }

        // POST: /LoginForEmployee
        [HttpPost]
        public ActionResult LoginEmployee(string email, string password)
        {
            Session["Picture"] = customerDao.getPictureStaff(email);

            // Check if the email and password are provided.
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Email and password are required";
                return View("LoginForEmployee");
            }

            // Check the user credentials.
            if (accountDao.IsValidUser(email, password))
            {
                // Set session and forms authentication cookie.
                Session["LoggedInUser"] = email;
                FormsAuthentication.SetAuthCookie(email, false);

                // Assuming `getByMaTK` is a method to get user details based on email.
                var user = customerDao.getByMaTK(email);
                if (user != null)
                {
                    Session["MaTK"] = user.MaTK;
                }

                // Determine if the user has an admin role.
                string role = accountDao.Role(email);
                Session["UserRole"] = role;

                // Redirect based on the user role.
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "ManagerAccount", new { area = "Admin" });
                }
                else if (role == "Nhân viên")
                {
                    return RedirectToAction("Index", "ManagerProduct", new { area = "Admin" });
                }
                else if (role == "Nhân viên kho")
                {
                    return RedirectToAction("Index", "ManagerWarehouse", new { area = "Admin" });
                }
                else
                {
                    // Return a default view or redirect if the role is unknown or not handled.
                    ViewBag.ErrorMessage = "Unauthorized access";
                    return View("LoginForEmployee"); // Adjust view as necessary
                }
            }
            else
            {
                // Authentication failed, show error message.
                ViewBag.ErrorMessage = "Invalid email or password";
                return View("LoginForEmployee"); // Assume "Index" is the login view.
            }
        }

        // POST: /LogoutEmployee
        [HttpPost]
        public ActionResult LogoutEmployee()
        {
            FormsAuthentication.SignOut();
            Session["MaTK"] = null;
            Session["UserRole"] = null;
            Session["LoggedInUser"] = null;
            return RedirectToAction("LoginForEmployee", "Account");
        }
        
        #endregion
    }
}
