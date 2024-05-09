using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KFC_Framework.Areas.admin
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Kiểm tra xem người dùng đã đăng nhập và có quyền admin hay không
            return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin");
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu không có quyền admin, chuyển hướng đến trang đăng nhập
            filterContext.Result = new RedirectResult("/Account/Index");
        }
    }
}