using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using WebMarkupMin.AspNet4.Mvc;

namespace MyEshop.Controllers
{
    [MinifyHtml]
    public class ProductController : Controller
    {
        MyEshop_DBEntities db;
        public ProductController()
        {
            db = new MyEshop_DBEntities();
        }
        public ActionResult ShowGrops()
        {
            //ViewBag.Count = db.Product_Selected_Groups.GroupBy(g => g.GroupID).Select(g => new Product_Groups_CountViewModel()
            //{
            //    GroupID = g.Key,
            //    Count = g.Count()
            //}).ToList();
            return PartialView("ShowGroups", db.Product_Groups.ToList());
        }
        public ActionResult LastProduct()
        {
            return PartialView("LastProduct", db.Products.OrderByDescending(p=>p.CreateDate).Take(6).ToList());
        }
        [Route("ShowProduct/{id}")]
        public ActionResult ShowProduct(int id)
        {
            var product = db.Products.Find(id);
            ViewBag.Feature = product. Product_Feature.DistinctBy(f => f.FeatureID).Select(f => new ShowProductFeatureViewModel()
            {
                FeatureTitle = f.Feature.FeatureTitle,
                Valuse = db.Product_Feature.Where(fe => fe.ProductID == f.ProductID && fe.FeatureID==f.FeatureID).Select(fe => fe.Value).ToList()
            }).ToList();
            if (product==null)
            {
                return HttpNotFound();
            }
            return View("ShowProduct", product);
        }
        public ActionResult ShowComment(int id)
        {
            return PartialView(db.Product_Comment.Where(c => c.ProductID == id).ToList());
        }

        public ActionResult CreateComment(int id)
        {
            return PartialView(new Product_Comment()
            {
                ProductID = id
            });
        }
       [HttpPost]
       public ActionResult CreateComment(Product_Comment product_Comment)
        {
            if (ModelState.IsValid)
            {
                product_Comment.CreateDate = DateTime.Now;
                db.Product_Comment.Add(product_Comment);
                db.SaveChanges();
                return PartialView("ShowComment", db.Product_Comment.Where(c => c.ProductID == product_Comment.ProductID));
            }
            return PartialView(product_Comment);
        }

        public ActionResult ShowProductByGroups(int id)
        {
            var product = db.Product_Selected_Groups.Where(g => g.GroupID == id).Select(g=>g.Product).ToList();
            return View(product);
        }
    }
}