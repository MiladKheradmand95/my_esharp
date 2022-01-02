using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InsertShowImage;
using DataLayer;
using System.IO;
using KooyWebApp_MVC.Classes;

namespace MyEshop.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private MyEshop_DBEntities db = new MyEshop_DBEntities();

        // GET: /Admin/Product/
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: /Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: /Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.groups = db.Product_Groups.ToList();
            return View();
        }

        // POST: /Admin/Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Title,ShortDescription,Text,Price,ImageName,CreateDate")] Product products, List<int> selectedGroups, HttpPostedFileBase ImageProduct,string tags)
        {
            if (ModelState.IsValid)
            {
                if (selectedGroups==null)
                {
                    ViewBag.Error = true;
                    ViewBag.groups = db.Product_Groups.ToList();
                    return View(products);
                }
                products.ImageName = "images.jpg";
                if (ImageProduct!=null && ImageProduct.IsImage())
                {
                    products.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(ImageProduct.FileName);
                    ImageProduct.SaveAs(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + products.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                }
                products.CreateDate = DateTime.Now;
                db.Products.Add(products);
                foreach (int  SelectedGroup in selectedGroups)
                {
                    db.Product_Selected_Groups.Add(new Product_Selected_Groups()
                    {
                        ProductID = products.ProductID,
                        GroupID = SelectedGroup
                    });
                }
                if (!string.IsNullOrEmpty(tags))
                {
                    string[] tag = tags.Split(',');
                    foreach (string t in tag)
                    {
                        db.Product_Tags.Add(new Product_Tags()
                        {
                            ProductID = products.ProductID,
                            Tag = t.Trim()
                        });
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.groups = db.Product_Groups.ToList();
            return View(products);
        }

        // GET: /Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedGroup = products.Product_Selected_Groups.ToList();
            ViewBag.Tags = string.Join(",", products.Product_Tags.Select(t => t.Tag).ToList());
            ViewBag.groups = db.Product_Groups.ToList();
            return View(products);
        }

        // POST: /Admin/Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ProductID,Title,ShortDescription,Text,Price,ImageName,CreateDate")] Product products, List<int> selectedGroups, HttpPostedFileBase ImageProduct, string tags)
        {
            if (ModelState.IsValid)
            {
                if (ImageProduct!=null &&ImageProduct.IsImage())
                {
                    if (products.ImageName != "images.jpg")
                    {
                        System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                        System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                    }
                    products.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(ImageProduct.FileName);
                    ImageProduct.SaveAs(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + products.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                } 
                db.Entry(products).State = EntityState.Modified;

                db.Product_Tags.Where(t => t.ProductID == products.ProductID).ToList().ForEach(t => db.Product_Tags.Remove(t));
                if (!string.IsNullOrEmpty(tags))
                {
                    string[] tag = tags.Split(',');
                    foreach (string t in tag)
                    {
                        db.Product_Tags.Add(new Product_Tags()
                        {
                            ProductID = products.ProductID,
                            Tag = t.Trim()
                        });
                    }
                }

                db.Product_Selected_Groups.Where(p => p.ProductID == products.ProductID).ToList().ForEach(p => db.Product_Selected_Groups.Remove(p));
                if (selectedGroups!=null&&selectedGroups.Any())
                {
                    foreach (int SelectedGroup in selectedGroups)
                    {
                        db.Product_Selected_Groups.Add(new Product_Selected_Groups()
                        {
                            ProductID = products.ProductID,
                            GroupID = SelectedGroup
                        });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SelectedGroup = selectedGroups;
            ViewBag.Tags = tags;
            ViewBag.groups = db.Product_Groups.ToList();
            return View(products);
        }

        // GET: /Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: /Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product==null)
            {
                HttpNotFound();
            }
            if (db.Product_Selected_Groups.Any(p=>p.ProductID==id))
            {
                var psg = db.Product_Selected_Groups.Where(p => p.ProductID == id);
                foreach (var item in psg)
                {
                    db.Product_Selected_Groups.Remove(item);
                }
            }

            if (db.Product_Feature.Any(pf=>pf.ProductID==id))
            {
                var pf = db.Product_Feature.Where(p => p.ProductID == id);
                foreach (var item in pf)
                {
                    db.Product_Feature.Remove(item);
                }
            }

            if (db.Product_Tags.Any(p => p.ProductID == id))
            {
                var psg = db.Product_Tags.Where(p => p.ProductID == id);
                foreach (var item in psg)
                {
                    db.Product_Tags.Remove(item);
                }
            }

            if (db.Product_Galleries.Any(p => p.ProductID == id))
            {
                var psg = db.Product_Galleries.Where(p => p.ProductID == id);
                foreach (var item in psg)
                {
                    db.Product_Galleries.Remove(item);
                }
            }
            var imagename = Server.MapPath("/Images/ProductImages/"+product.ImageName);
            if (System.IO.File.Exists(imagename))
            {
                System.IO.File.Delete(imagename);
            }

            var imagePath = Server.MapPath("/Images/ProductImages/Thumb/" + product.ImageName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            db.Products.Remove(product);
            //db.Product_Selected_Groups.Remove(psg);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Gallery
        public ActionResult Gallery(int id)
        {
            ViewBag.Galleries = db.Product_Galleries.Where(pg => pg.ProductID == id).ToList();
            return View("Gallery", new Product_Galleries()
            {
                ProductID = id
            });
        }

        [HttpPost]
        public ActionResult Gallery(Product_Galleries gallery, HttpPostedFileBase imgUp)
        {
            if (ModelState.IsValid)
            {
                if (imgUp != null && imgUp.IsImage())
                {
                    gallery.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imgUp.FileName);
                    imgUp.SaveAs(Server.MapPath("/Images/ProductImages/" + gallery.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + gallery.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + gallery.ImageName));
                    db.Product_Galleries.Add(gallery);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("Gallery", new { id = gallery.ProductID });
        }

        public ActionResult DeleteGallery(int id)
        {
            var gallery = db.Product_Galleries.Find(id);
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + gallery.ImageName));
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + gallery.ImageName));
            db.Product_Galleries.Remove(gallery);
            db.SaveChanges();
            return RedirectToAction("Gallery", new { id = gallery.ProductID });
        }
        #endregion

        #region Product_Feature
        public ActionResult Product_Feature(int id)
        {
            ViewBag.Feature = db.Product_Feature.Where(f => f.ProductID == id).ToList();
            ViewBag.FeatureID = new SelectList(db.Features, "FeatureID", "FeatureTitle");
            return View(new Product_Feature()
            {
                ProductID=id
            });
        }

        [HttpPost]
        public ActionResult Product_Feature(Product_Feature product_Feature)
        {
            if (ModelState.IsValid)
            {
                db.Product_Feature.Add(product_Feature);
                db.SaveChanges();
            }
            return RedirectToAction("Product_Feature", new { id = product_Feature.ProductID });
        }

        public void Delete_Product_Feature(int id)
        {
            var res = db.Product_Feature.Find(id);
            db.Product_Feature.Remove(res);
            db.SaveChanges();
        }
        #endregion


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
