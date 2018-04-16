using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelApplication.Models;
using TravelApplication.Services;

namespace TravelApplication.Controllers.MVC
{
    public class TravelRequestController : Controller
    {
        [HttpGet]
        public ActionResult Approve(string travelRequestId, int badgeNumber)
        {
            var approveModel = new ApproveModel()
            {
                TravelRequestId = travelRequestId,
                BadgeNumber     = badgeNumber
            };

            return View("Approve", approveModel);
        }

        
    }
}