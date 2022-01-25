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
    public class SearchController : Controller
    {
        MyEshop_DBEntities db = new MyEshop_DBEntities();
        // GET: Search
        public ActionResult Index(string q)
        {
            List<Product> list = new List<Product>();
            list.AddRange(db.Product_Tags.Where(t => t.Tag == q).Select(t=>t.Product));
            list.AddRange(db.Products.Where(p => p.Title.Contains(q) || p.ShortDescription.Contains(q) || p.Text.Contains(q)).ToList());
            ViewBag.Search = q;
            return View(list.Distinct());
        }
    }
}