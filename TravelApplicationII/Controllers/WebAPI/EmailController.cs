using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TravelApplication.Class.Common;
using TravelApplication.Models;
using TravelApplication.Services;

namespace TravelApplication.Controllers.WebAPI
{
    public class EmailController : ApiController
    {
        IEmailService emailService = new EmailService();

        [HttpPost]
        [Route("api/email/sendemail")]
        public HttpResponseMessage Email(Models.Email email)
        {
            HttpResponseMessage response = null;

            try
            {

                var result = emailService.SendEmail(email.FromAddress , email.ToAddress, email.Subject,email.Body).ConfigureAwait(false);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/email/sendemail : " + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Could not send an email.");
            }

            return response;
        }
    }
}
