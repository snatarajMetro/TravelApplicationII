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
    public class TravelRequestController : ApiController
    {
        ITravelRequestService travelRequestService = new TravelRequestService();

        [HttpGet]
        [Route("api/travelrequest/employee/{badgeNumber}")]
        public HttpResponseMessage GetEmployeeDetails(int badgeNumber)
        {
            HttpResponseMessage response = null;
            try
            {
                var result = travelRequestService.GetEmployeeDetails(badgeNumber).Result;
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetEmployeeDetails :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(@"The badge# {0} you have entered is invalid. Please try again.", badgeNumber));

            }
            return response;
        }

        [HttpPost]
        [Route("api/travelrequest/save")]
        public HttpResponseMessage Save(TravelRequest travelRequest)
        {
            HttpResponseMessage response = null;
            try
            {
                var result = travelRequestService.SaveTravelRequest(travelRequest).Result;
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequest/save :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't save travel request : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/travelrequest/{travelRequestId}")]
        public HttpResponseMessage GetTravelRequestDetails(int travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {
                TravelRequest result = travelRequestService.GetTravelRequestDetail(travelRequestId);

                //TravelRequest result = new TravelRequest()
                //{
                //    BadgeNumber                 = 10005,
                //    DepartureDateTime           = DateTime.Now,
                //    MeetingBeginDateTime        = DateTime.Now.AddDays(3),
                //    MeetingEndDateTime          = DateTime.Now.AddDays(4),
                //    MeetingLocation             = "Downtown LA",
                //    Organization                = "LA Metro",
                //    ReturnDateTime              = DateTime.Now.AddDays(4),
                //    SubmittedByBadgeNumber      = 10005,
                //    SubmittedDateTime           = DateTime.Now,
                //    TravelRequestId             = travelRequestId,
                //    Name                        = "Nataraj.S",
                //    Division                    = "Development",
                //    Section                     = "IT"                  
                //};
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetails :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrive travel request details for the given travel request Id : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/travelrequests")]
        public HttpResponseMessage GetTravelrequestList(int badgeNumber, int roleId)
        {

            HttpResponseMessage response = null;
            try
            {
                var result = travelRequestService.GetTravelrequestList(badgeNumber, roleId);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelrequestList :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve EmployeeInfo for Badge # : " + ex.Message);

            }
            return response;
        }

        [HttpPost]
        [Route("api/travelrequest/approve")]
        public HttpResponseMessage Approve(ApproveRequest approveRequest)
        {
            HttpResponseMessage response = null;

            try
            {

                var result = travelRequestService.Approve(approveRequest.ApproverBadgeNumber, approveRequest.TravelRequestId, approveRequest.Comments);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequest/approve :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Approve was not sucessfull. Please try again.");
            }

            return response;
        }

        [HttpPost]
        [Route("api/travelrequest/reject")]
        public HttpResponseMessage Reject(ApproveRequest approveRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = travelRequestService.Reject(approveRequest);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequest/reject :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Reject was not sucessfull. Please try again.");
            }

            return response;
        }

        [HttpPost]
        [Route("api/travelrequest/saveNew")]
        public HttpResponseMessage Save(TravelRequestInput travelRequest)
        {
            HttpResponseMessage response = null;
            try
            {
                var result = travelRequestService.SaveTravelRequestInput(travelRequest);
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequest/saveNew :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't save travel request : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/travelrequestNew/{travelRequestId}")]
        public HttpResponseMessage GetTravelRequestDetailsNew(string travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {
                TravelRequestInput result = travelRequestService.GetTravelRequestDetailNew(travelRequestId);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetailsNew:" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrive travel request details for the given travel request Id : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/travelrequest/submitdetails/{travelRequestId}")]
        public HttpResponseMessage GetSubmitDetails(int travelRequestId)
        {
            HttpResponseMessage response = null;
            TravelRequestSubmitDetailResponse result = null;
            try
            {
                result = travelRequestService.GetSubmitDetails(travelRequestId);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetSubmitDetails:" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [HttpPost]
        [Route("api/travelrequest/cancel")]
        public HttpResponseMessage Cancel(ApproveRequest approveRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = travelRequestService.Cancel(approveRequest.TravelRequestId, approveRequest.TravelRequestBadgeNumber, approveRequest.Comments);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/travelrequest/cancel:" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Cancel was not sucessfull. Please try again.");
            }

            return response;
        }

        [HttpGet]
        [Route("api/travelrequest/dashboard")]
        public HttpResponseMessage GetTravelRequestDashboard()
        {
            HttpResponseMessage response = null;

            var result = travelRequestService.GetTravelRequestDashboardData();
            //var result = new List<TravelRequestDashboard>();
            //result.Add(new TravelRequestDashboard() { Label = "New", Color = "orange", Value = 19 });
            //result.Add(new TravelRequestDashboard() { Label = "Pending", Color = "dodgerblue", Value = 0 });
            //result.Add(new TravelRequestDashboard() { Label = "Rejected", Color = "red", Value = 0 });
            //result.Add(new TravelRequestDashboard() { Label = "Completed", Color = "green", Value = 2 });
            //result.Add(new TravelRequestDashboard() { Label = "Cancelled", Color = "purple", Value = 0 });

            var data = new JavaScriptSerializer().Serialize(result);

            response = Request.CreateResponse(HttpStatusCode.OK, data);

            return response;
        }

        [HttpGet]
        [Route("api/travelreimbursement/dashboard")]
        public HttpResponseMessage GetTravelReimbursementtDashboard()
        {
            HttpResponseMessage response = null;
            var result = travelRequestService.GetTravelReimbursementDashboardData();
            //var result = new List<TravelReimbursementDashboard>();
            //result.Add(new TravelReimbursementDashboard() { Label = "New", Color = "orange", Value = 9 });
            //result.Add(new TravelReimbursementDashboard() { Label = "Pending", Color = "dodgerblue", Value = 35 });
            //result.Add(new TravelReimbursementDashboard() { Label = "Rejected", Color = "red", Value = 3 });
            //result.Add(new TravelReimbursementDashboard() { Label = "Completed", Color = "green", Value = 12 });
            //result.Add(new TravelReimbursementDashboard() { Label = "Cancelled", Color = "purple", Value = 7 });

            var data = new JavaScriptSerializer().Serialize(result);

            response = Request.CreateResponse(HttpStatusCode.OK, data);

            return response;
        }
    }


}

  
    