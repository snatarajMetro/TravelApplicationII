using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using TravelApplication.Class.Common;
using TravelApplication.Models;
using TravelApplication.Services;

namespace TravelApplication.Controllers.WebAPI
{
    public class FISController : ApiController
    {
        IFISService fisService = new FISService();

        [HttpGet]
        [Route("api/fis/costcenters")]
        public HttpResponseMessage GetCostCenters()
        {
            HttpResponseMessage response = null;
            try
            {
                var result = fisService.GetCostCenters().Result;
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/fis/costcenters :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve cost centers from FIS  " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/fis/projects/{costCenterName}")]
        public HttpResponseMessage GetProjectsByCostCenterName(string costCenterName)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = fisService.GetProjectsByCostCenterName(costCenterName).Result;
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetProjectsByCostCenterName :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve projects by cost center Id  " + ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("api/fis/delay")]
        public HttpResponseMessage Delay()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
