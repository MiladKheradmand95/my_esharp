using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataLayer;

namespace MyEshop.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private MyEshop_DBEntities db = new MyEshop_DBEntities();

        // GET: /Admin/Users/
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        // GET: /Admin/Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: /Admin/Users/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleTitle");
            return View();
        }

        // POST: /Admin/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UserID,RoleID,UserName,Email,Password,ActiveCode,IsActive,RegisterDate")] User users)
        {
            if (ModelState.IsValid)
            {
                users.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(users.Password, "MD5");
                users.ActiveCode = Guid.NewGuid().ToString();
                users.RegisterDate = DateTime.Now;
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleTitle", users.RoleID);
            return View(users);
        }

        // GET: /Admin/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleTitle", users.RoleID);
            return View(users);
        }

        // POST: /Admin/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UserID,RoleID,UserName,Email,Password,ActiveCode,IsActive,RegisterDate")] User users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("/Index");
            }
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleTitle", users.RoleID);
            return View(users);
        }

        // GET: /Admin/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: /Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            bool res=db.Users.Where(u => u.UserName == User.Identity.Name).Any();
            var count=db.Users.Count();
            if (res==false || count==0)
            {
                return Redirect("/Account/LogOff");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
