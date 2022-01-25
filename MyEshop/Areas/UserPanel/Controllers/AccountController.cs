using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using DataLayer;
using DataLayer.ViewModels;
using WebMarkupMin.AspNet4.Mvc;
using System.Net;
using Newtonsoft.Json;
using MyEshop.Utilities;
using System.IO;

namespace MyEshop.Areas.UserPanel.Controllers
{
    [MinifyHtml]
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
        
        public ActionResult ChangePassword(FormCollection form, ChangePasswordViewModel change)
        {

            string urlToPost = "https://www.google.com/recaptcha/api/siteverify";
            string secretKey = "6LfxmiIeAAAAAFSKZCdxCqgQaeOk3tWGzRJKcD0o"; // change this
            string gRecaptchaResponse = form["g-recaptcha-response"];

            var postData = "secret=" + secretKey + "&response=" + gRecaptchaResponse;

            // send post data
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToPost);
            request.Method = "POST";
            request.ContentLength = postData.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(postData);
            }

            // receive the response now
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            // validate the response from Google reCaptcha
            var captChaesponse = JsonConvert.DeserializeObject<reCaptchaResponse>(result);
            if (!captChaesponse.Success)
            {
                ViewBag.Message = "Sorry, please validate the reCAPTCHA";
                return View();
            }
            // go ahead and write code to validate username password against database
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