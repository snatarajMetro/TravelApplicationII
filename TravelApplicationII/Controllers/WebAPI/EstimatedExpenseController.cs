using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using TravelApplication.Class.Common;
using TravelApplication.Models;
using TravelApplication.Services;

namespace TravelApplication.Controllers.WebAPI
{
    public class EstimatedExpenseController : ApiController
    {
        IEstimatedExpenseService estimatedExpenseService = new EstimatedExpenseService();
        [HttpPost]
        [Route("api/estimatedexpense/save")]
        public HttpResponseMessage Save(EstimatedExpense estimatedExpense)
        {
            HttpResponseMessage response = null;
            try
            {
                var result = estimatedExpenseService.SaveEstimatedExpenseRequest(estimatedExpense).Result;
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/estimatedexpense/save :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't save travel request : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/estimatedexpense/{travelRequestId}")]
        public HttpResponseMessage GetEstimatedExpenseByTravelRequestId(int travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {
                EstimatedExpense result = estimatedExpenseService.GetEstimatedExpenseByTravelRequestId(travelRequestId);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetEstimatedExpenseByTravelRequestId :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrive travel request details for the given travel request Id : " + ex.Message);

            }
            return response;
        }

    }
}
