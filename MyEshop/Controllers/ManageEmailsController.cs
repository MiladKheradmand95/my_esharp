using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMarkupMin.AspNet4.Mvc;

namespace MyEshop.Controllers
{
    [MinifyHtml]
    public class ManageEmailsController : Controller
    {
        //
        // GET: /ManageEmails/
        public ActionResult ActiviationEmail()
        {
            return PartialView();
        }

        public ActionResult RecoveryPassword()
        {
            return PartialView();
        }
	}
}