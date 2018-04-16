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
    public class ApprovalController : ApiController
    {
        IApprovalService approvalService = new ApprovalService();
        [HttpGet]
        [Route("api/approval/HeirarchichalPositions/{badgeNumber}")]
        public HttpResponseMessage GetHeirarchichalPositions(int badgeNumber)
        {
            HttpResponseMessage response = null;
            try
            {
                var result = approvalService.GetHeirarchichalPositions(badgeNumber).Result;

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetHeirarchichalPositions :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve heirarchichal information from FIS  " + ex.Message);

            }
            return response;


        }

        [HttpGet]
        [Route("api/approval/TAApprovers")]
        public HttpResponseMessage GetTAAprovers()
        {
            HttpResponseMessage response = null;
            try
            {
                var result = approvalService.GetTAAprovers();
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTAAprovers :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve Travel admin approvers  " + ex.Message);

            }
            return response;


        }

        [HttpPost]
        [Route("api/approval/submit")]
        public HttpResponseMessage SubmitTravelRequest(SubmitTravelRequestData submitTravelRequestData)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = approvalService.SubmitTravelRequest(submitTravelRequestData);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("SubmitTravelRequest :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Travel request was not successfully submited. Please try again.");
            }

            return response;
        }

        [HttpPost]
        [Route("api/approval/submitNew")]
        public HttpResponseMessage SubmitTravelRequestNew(SubmitTravelRequest submitTravelRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = approvalService.SubmitTravelRequestNew(submitTravelRequest);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/approval/submitNew :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Travel request was not successfully submited. Please try again.");
            }

            return response;
        }

        [HttpGet]
        [Route("api/approval/approverDetails/{travelRequestId}")]
        public HttpResponseMessage GetapproverDetails(string travelRequestId)
        {
            HttpResponseMessage response = null;
            try
            {
                SubmitTravelRequest  result = approvalService.GetapproverDetails(travelRequestId);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetapproverDetails :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Couldn't retrieve approver information " + ex.Message);

            }
            return response;
        }

        [HttpPost]
        [Route("api/approval/submitReimburse")]
        public HttpResponseMessage SubmitReimburse(SubmitReimburseData submitReimburseData)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = approvalService.SubmitReimburse(submitReimburseData);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("SubmitReimburse :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Travel request was not successfully submited. Please try again.");
            }

            return response;
        }

        //Email approval calls this api
        [HttpPost]
        [Route("api/approval/Approve")]
        public HttpResponseMessage Approve(EmailApprovalDetails emailApproveDetails)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = approvalService.UpdateApproveStatus(emailApproveDetails);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/approval/Approve :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Travel request was not successfully approved.");
            }

            return response;
        }

        [HttpPost]
        [Route("api/approval/Reject")]
        public HttpResponseMessage Reject(EmailApprovalDetails emailApproveDetails)
        {
            HttpResponseMessage response = null;

            try
            {
                var result = approvalService.UpdateRejectStatus(emailApproveDetails);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/approval/Reject :" + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Rejection failed");
            }

            return response;
        }
    }
}
