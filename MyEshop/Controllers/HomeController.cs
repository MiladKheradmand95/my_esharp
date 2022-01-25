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
    public class HomeController : Controller
    {
        MyEshop_DBEntities db = new MyEshop_DBEntities();
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Slider()
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            return PartialView(db.Sliders.Where(s=>s.IsActive==true && s.StartDate <= dt && s.EndDate >= dt).ToList());
        }

        [Route("AboutUS")]
        public ActionResult AboutUS()
        {
            return View();
        }
        public ActionResult VisitSite()
        {
            DateTime dtTodey = DateTime.Now.Date;
            DateTime dtYseterday = dtTodey.AddDays(-1);
            VisitSiteViewModel visit = new VisitSiteViewModel();
            visit.VisitSum = db.SiteVisits.Count();
            visit.VisitTodey = db.SiteVisits.Count(v => v.Date == dtTodey);
            visit.VisitYesterday = db.SiteVisits.Count(v => v.Date == dtYseterday);
            visit.Online = int.Parse(HttpContext.Application["Online"].ToString());
            return PartialView(visit);
        }
	}
}