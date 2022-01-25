using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using InsertShowImage;
using KooyWebApp_MVC.Classes;
using WebMarkupMin.AspNet4.Mvc;

namespace MyEshop.Areas.Admin.Controllers
{
    [MinifyHtml]
    public class SlidersController : Controller
    {
        private MyEshop_DBEntities db;
        public SlidersController()
        {
               db = new MyEshop_DBEntities();
        }
         
        // GET: Admin/Sliders
        public ActionResult Index()
        {
            return View(db.Sliders.ToList());
        }

        // GET: Admin/Sliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SliderID,Title,Url,ImageName,StartDate,EndDate,IsActive")] Slider slider,HttpPostedFileBase ImgUp)
        {
            if (ModelState.IsValid)
            {
                if (ImgUp==null && ImgUp.IsImage())
                {
                    ModelState.AddModelError("ImageName", "لطفا تصویر مورد نظر را انتخاب کنید");
                    return View(slider);
                }
                slider.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(ImgUp.FileName);
                ImgUp.SaveAs(Server.MapPath("/Images/Slider/" + slider.ImageName));
                ImageResizer img = new ImageResizer();
                img.Resize(Server.MapPath("/Images/Slider/" + slider.ImageName), Server.MapPath("/Images/Slider/Thumb/" + slider.ImageName));
                db.Sliders.Add(slider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SliderID,Title,Url,ImageName,StartDate,EndDate,IsActive")] Slider slider,HttpPostedFileBase ImgUp)
        {
            if (ModelState.IsValid)
            {
                if (ImgUp!=null)
                {
                    System.IO.File.Delete(Server.MapPath("/Images/Slider/" + slider.ImageName));
                    System.IO.File.Delete(Server.MapPath("/Images/Slider/Thumb/" + slider.ImageName));
                    slider.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(ImgUp.FileName);
                    ImgUp.SaveAs(Server.MapPath("/Images/Slider/" + slider.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/Slider/" + slider.ImageName), Server.MapPath("/Images/Slider/Thumb/" + slider.ImageName));
                }
                db.Entry(slider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        // GET: Admin/Sliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slider slider = db.Sliders.Find(id);
            db.Sliders.Remove(slider);
            db.SaveChanges();
            System.IO.File.Delete(Server.MapPath("/Images/Slider/" + slider.ImageName));
            System.IO.File.Delete(Server.MapPath("/Images/Slider/Thumb/" + slider.ImageName));
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
