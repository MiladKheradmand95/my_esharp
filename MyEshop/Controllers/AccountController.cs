using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataLayer.ViewModels;
using DataLayer;
using WebMarkupMin.AspNet4.Mvc;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using MyEshop.Utilities;

namespace MyEshop.Controllers
{
    [MinifyHtml]
    public class AccountController : Controller
    {
        MyEshop_DBEntities db = new MyEshop_DBEntities();
        //private IUsersRepository UserRepository;
        //public AccountController()
        //{
        //    UserRepository = new UserRepositories(db);
        //}
        //
        // GET: /Account/
        [Route("Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public ActionResult Register(FormCollection form,RegisterviewModel register)
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
                if (!db.Users.Any(u => u.Email == register.Email.Trim().ToLower()))
                {
                    User user = new User()
                    {
                        Email = register.Email.Trim().ToLower(),
                        UserName = register.UserName,
                        Password = FormsAuthentication.HashPasswordForStoringInConfigFile(register.Password, "MD5"),
                        ActiveCode = Guid.NewGuid().ToString(),
                        IsActive = false,
                        RoleID = 1,
                        RegisterDate = DateTime.Now
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    string body = PartialToStringClass.RenderPartialView("ManageEmails", "ActiviationEmail", user);
                    SendEmail.Send(user.Email, "ایمیل فعالسازی", body);
                    return View("SuccessRegister", user);
                }
                else
                {
                    ModelState.AddModelError("Email", "ایمیل وارده تکراری میباشد");
                }
            }
            return View();
        }

        public ActionResult ActiveUser(string id)
        {
            var user = db.Users.SingleOrDefault(u => u.ActiveCode == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserName = user.UserName;
            user.IsActive = true;
            user.ActiveCode = Guid.NewGuid().ToString();
            db.SaveChanges();
            return View();
        }

        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(FormCollection form,LoginViewModels login, string ReturnUrl = "/")
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
                string hashPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(login.Password, "MD5");
                var user = db.Users.SingleOrDefault(u => u.Email == login.Email && u.Password == hashPassword);
                if (user != null)
                {
                    if (user.IsActive == true)
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, login.RememberMe);
                        //ViewBag.IsSuccess = true;
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "حساب کاربری شما فعال نشده است");

                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
                }
            }
            return View();
        }


        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        [Route("ForgetPassword")]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public ActionResult ForgetPassword(FormCollection form, ForgetPassword forget)
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
                var user = db.Users.SingleOrDefault(u => u.Email == forget.Email);
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        string RecoveryEmailBody = PartialToStringClass.RenderPartialView("ManageEmails", "RecoveryPassword", user);
                        SendEmail.Send(user.Email, "بازیابی کلمه عبور", RecoveryEmailBody);
                        return View("SuccessForgotPassword", user);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "حساب کاربری شما فعال نشده است");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
                }
            }
            return View();
        }

        public ActionResult RecoveryPassword(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecoveryPassword(string id, RecoveryPassword recovery)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.ActiveCode == id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(recovery.Password, "MD5");
                user.ActiveCode = Guid.NewGuid().ToString();
                db.SaveChanges();
                return Redirect("/Login?Recovery=true");
            }

            return View();
        }
       
    }

}