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
    public class ReimbursementController : ApiController
    {
        IReimbursementService reimbursementService = new ReimbursementService();
        // Reimbursement section 
        [HttpGet]
        [Route("api/reimburse/approvedTravelrequests")]
        public HttpResponseMessage GetapprovedTravelrequestList(int badgeNumber, int roleId)
        {

            HttpResponseMessage response = null;
            try
            {
                var result = reimbursementService.GetApprovedTravelrequestList(badgeNumber, roleId);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/reimburse/approvedTravelrequests :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve EmployeeInfo for Badge # : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/reimburse/TravelrequestDetails/{travelRequestId}")]
        public HttpResponseMessage GetapprovedTravelrequestList(string travelRequestId)
        {

            HttpResponseMessage response = null;
            try
            {
                var result = reimbursementService.GetTravelRequestInfoForReimbursement(travelRequestId);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/reimburse/TravelrequestDetails/{travelRequestId} :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve EmployeeInfo for Badge # : " + ex.Message);

            }
            return response;
        }

        [HttpPost]
        [Route("api/reimburse/save")]
        public HttpResponseMessage SaveReimbursement(ReimbursementInput reimbursementRequest)
        {
            HttpResponseMessage response = null;
            try
            {
                var result = reimbursementService.SaveTravelRequestReimbursement(reimbursementRequest);
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/reimburse/save :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't save travel request : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/reimburse/reimbursementRequests")]
        public HttpResponseMessage GetReimbursementRequests(int badgeNumber, int roleId)
        {

            HttpResponseMessage response = null;
            try
            {
                var result = reimbursementService.GetReimbursementRequests(badgeNumber, roleId);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/reimburse/reimbursementRequests" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve EmployeeInfo for Badge # : " + ex.Message);

            }
            return response;
        }

        [HttpGet]
        [Route("api/reimburse/submitdetails/{travelRequestId}")]
        public HttpResponseMessage GetSubmitDetails(int travelRequestId)
        {
            HttpResponseMessage response = null;
            TravelRequestSubmitDetailResponse result = null;
            try
            {
                result = reimbursementService.GetSubmitDetails(travelRequestId);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetSubmitDetails:" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [HttpGet]
        [Route("api/reimburse/{travelRequestId}")]
        public HttpResponseMessage GetReimbursementDetails(string travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {
                ReimbursementInput result = reimbursementService.GetReimbursementDetails(travelRequestId);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api / reimburse /{ travelRequestId} :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrive reiumbursement details for the given  Id : " + ex.Message);

            }
            return response;
        }

        [HttpPost]
        [Route("api/reimburse/approve")]
        public HttpResponseMessage Approve(ApproveRequest approveRequest)
        {
            HttpResponseMessage response = null;

            try
            {

                var result = reimbursementService.Approve(approveRequest.ApproverBadgeNumber, approveRequest.TravelRequestId, approveRequest.Comments);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/reimburse/approve :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Approve was not sucessfull. Please try again.");
            }

            return response;
        }

        [HttpPost]
        [Route("api/reimburse/reject")]
        public HttpResponseMessage Reject(ApproveRequest approveRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                
                var result = reimbursementService.Reject(approveRequest.ApproverBadgeNumber, approveRequest.TravelRequestBadgeNumber ,approveRequest.TravelRequestId, approveRequest.Comments,approveRequest.RejectReason);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/reimburse/reject :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Reject was not sucessfull. Please try again.");
            }

            return response;
        }

    }
}
