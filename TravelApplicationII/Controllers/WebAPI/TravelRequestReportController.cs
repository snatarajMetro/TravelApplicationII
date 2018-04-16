using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TravelApplication.Class.Common;
using TravelApplication.Services;

namespace TravelApplication.Controllers.WebAPI
{
    public class TravelRequestReportController : ApiController
    {
        TravelRequestReportService travelRequestReportService = new TravelRequestReportService();

        [HttpGet]
        [Route("api/travelrequestReport/{travelRequestId}")]
        public HttpResponseMessage GetTravelRequestDetailsNew(string travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {
                LogMessage.Log("Starting Crystal Report");

                var byteContent = travelRequestReportService.RunReport("Travel_Request.rpt", "test", travelRequestId);

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(byteContent);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequestReport/{travelRequestId} - Travel Request :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrive travel request report for the given travel request Id : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/travelReimbursementReport/{travelRequestId}")]
        public HttpResponseMessage GetTravelReimbursementReport(string travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {

                var byteContent = travelRequestReportService.RunReport("Travel_Business_Expense.rpt", "test", travelRequestId);

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(byteContent);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequestReport/{travelRequestId} - Reimbursement:" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrive reimbursement request report for the given travel request Id : " + ex.Message);

            }
            return response;
        }
    }

}
