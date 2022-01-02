using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using DataLayer;
using DataLayer.ViewModels;

namespace MyEshop.Areas.UserPanel.Controllers
{
    public class AccountController : Controller
    {
        MyEshop_DBEntities db ;
        public AccountController()
        {
            db = new MyEshop_DBEntities();
        }
        //
        // GET: /UserPanel/Account/
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult ChangePassword(ChangePasswordViewModel change)
        {
            if (ModelState.IsValid)
            {
                string OldPasswordHash=FormsAuthentication.HashPasswordForStoringInConfigFile(change.OldPassword,"MD5");
                var user = db.Users.Single(u => u.UserName == User.Identity.Name);
                if (user.Password==OldPasswordHash)
                {
                    string NewPasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(change.Password, "MD5");
                    user.Password = NewPasswordHash;
                    db.SaveChanges();
                    ViewBag.Success = true;
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "کاربری با این مشخصات یافت نشد");
                }
            }
            
            return View();
        }
	}
}