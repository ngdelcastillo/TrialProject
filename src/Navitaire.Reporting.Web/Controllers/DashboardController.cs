using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Navitaire.Reporting.Web.Models;

namespace Navitaire.Reporting.Web.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            DashboardModel model = new DashboardModel();
            model.Iframe = "http://nvdcmbm101:8282/";
            return View(model);
        }

    }
}
