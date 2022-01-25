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
    public class ArchiveController : Controller
    {
        MyEshop_DBEntities db = new MyEshop_DBEntities();
        // GET: Archive
        [Route("Archive")]
        public ActionResult ArchiveProduct(int pageId = 0, string title = "", int minPrice = 0, int maxPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.Groups = db.Product_Groups.ToList();
            ViewBag.ProductTitle = title;
            ViewBag.ProductMinPrice = minPrice;
            ViewBag.ProductMaxPrice = maxPrice;
            ViewBag.ProductGroups = selectedGroups;
            ViewBag.pageId = pageId;
            List<Product> list = new List<Product>();
            if (selectedGroups != null&&selectedGroups.Any())
            {
                foreach (var item in selectedGroups)
                {
                    list.AddRange(db.Product_Selected_Groups.Where(g => g.GroupID == item).Select(g => g.Product).ToList());
                }
                list = list.Distinct().ToList();
            }
            else
            {
                list.AddRange(db.Products.ToList());
            }
            if (title != "")
            {
                list = list.Where(l => l.Title.Contains(title)).ToList();
            }
            if (minPrice > 0)
            {
                list = list.Where(l => l.Price >= minPrice).ToList();
            }
            if (maxPrice > 0)
            {
                list = list.Where(l => l.Price <= maxPrice).ToList();
            }

            //Paging....
            int take = 9;
            int skip =(pageId - 1) * take;
            double count= (double)list.Count / (double)take ;
            ViewBag.PageCount = Math.Ceiling(count);
            return View("ArchiveProduct", list.OrderByDescending(l=>l.CreateDate).Skip(skip).Take(take).ToList());
        }
    }
}