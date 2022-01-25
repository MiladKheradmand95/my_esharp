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
    public class CompareController : Controller
    {
        MyEshop_DBEntities db;
        public CompareController()
        {
            db = new MyEshop_DBEntities();
        }
        // GET: Compare
        public ActionResult Index()
        {
            List<CompareItem> list = new List<CompareItem>();
            if (Session["Compare"]!=null)
            {
                list = (List<CompareItem>)Session["Compare"];
            }
            List<Feature> features = new List<Feature>();
            List<Product_Feature> product_Features = new List<Product_Feature>();
            foreach (var item in list)
            {
                features.AddRange(db.Product_Feature.Where(p => p.ProductID == item.ProductID).Select(f => f.Feature).ToList());
                product_Features.AddRange(db.Product_Feature.Where(p => p.ProductID == item.ProductID).ToList());
            }
            ViewBag.features = features.Distinct().ToList();
            ViewBag.product_Features = product_Features;
            return View(list);
        }

        public ActionResult AddToCompare(int id)
        {
            List<CompareItem> list = new List<CompareItem>();
            if (Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;
            }

            if (!list.Any(p => p.ProductID == id))
            {
                var product = db.Products.Where(p => p.ProductID == id).Select(p => new { p.Title, p.ImageName }).Single();
                list.Add(new CompareItem()
                {
                    ProductID = id,
                    Title = product.Title,
                    ImageName = product.ImageName
                });
            }

            Session["Compare"] = list;
            return PartialView("ListCompare", list);
        }

        public ActionResult ListCompare()
        {
            List<CompareItem> list = new List<CompareItem>();
            if (Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;
            }
            return PartialView(list);
        }

        public ActionResult DeleteFromCompare(int id)
        {
            List<CompareItem> list = new List<CompareItem>();
            if (Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;

                int index = list.FindIndex(p => p.ProductID == id);
                list.RemoveAt(index);
                Session["Compare"] = list;
            }
            return PartialView("ListCompare", list);
        }

    }
}