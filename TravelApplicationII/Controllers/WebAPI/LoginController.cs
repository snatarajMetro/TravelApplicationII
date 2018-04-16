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
    public class LoginController : ApiController
    {

        [HttpPost]
        [Route("api/login")]
        public HttpResponseMessage Login(UserModel userModel)
        {
            IUserService userService = new UserService();
            HttpResponseMessage response = null;
            
            try
            {

                if(userModel == null || string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
                {
                    throw new Exception("Invalid username and/or password. Please try again.");
                }
                // AD authentication
                //if(userService.IsValidADUser(userModel.UserName, userModel.Password))
                //{
                var result = userService.ValidateAndGetRoles(userModel);
                var data = new JavaScriptSerializer().Serialize(result);

                response = Request.CreateResponse(HttpStatusCode.OK, data);
                //}
                //else
                //{
                //    throw new Exception ("  AD User ";
                //}

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/login : " + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            return response;
        }
    }


}
