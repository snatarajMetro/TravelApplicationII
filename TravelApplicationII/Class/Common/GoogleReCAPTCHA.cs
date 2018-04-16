using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;

namespace TravelApplication.Class.Common
{
    public class GoogleReCAPTCHA
    {
        public static string secretKey = ConfigurationManager.AppSettings.Get("recaptchaSecret");

        /// <summary>
        /// Validates response token with Google API
        /// </summary>
        /// <param name="responseToken">response value which is the value of g-recaptcha-response on the form</param>
        /// <returns>Return true if valid; otherwise return false</returns>
        public static bool Validate(string responseToken)
        {
            var webClient = new WebClient();
            var googleReply = webClient.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, responseToken));
            var googleReplyObj = JObject.Parse(googleReply);

            return (bool)googleReplyObj.SelectToken("success");
        }
    }
}