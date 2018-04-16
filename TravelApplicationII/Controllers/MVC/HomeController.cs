using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace TravelApplication.Controllers
{
    /// <summary>
    /// Sample MVC 5 controller
    /// </summary>
    public class HomeController : Controller
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(HomeController));

        public ActionResult Index()
        {
            logger.Info("Home clicked...");
            return View();
        }

        public ActionResult About()
        {
            logger.Info("About clicked...");
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            logger.Info("Contact clicked...");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}