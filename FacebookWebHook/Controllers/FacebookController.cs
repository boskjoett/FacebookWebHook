using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FacebookWebHook.Controllers
{
    /// <summary>
    /// Facebook Callback URL handler.
    /// In the App Dashboard at https://developers.facebook.com add a webhook subscription for 
    /// the Facebook page and set the callback URL to https://[hostname]/webhook/facebook
    /// /// See: https://developers.facebook.com/docs/graph-api/webhooks
    /// </summary>

    [Route("webhook/facebook")]
    public class FacebookController : Controller
    {
        // Action handler for GET requests from Facebook
        [HttpGet]
        public IActionResult Get(
            [FromQuery(Name="hub.mode")] string mode,
            [FromQuery(Name="hub.challenge")] string challenge,
            [FromQuery(Name="hub.verify_token")] string verifyToken)
        {
            if (string.IsNullOrEmpty(mode) ||
                string.IsNullOrEmpty(challenge) ||
                string.IsNullOrEmpty(verifyToken) ||
                mode != "subscribe")
            {
                return Ok();
            }

            if (verifyToken == "myfacebooktoken")
            {
                // The verify token is defined in the Facebook App Dashboard where you setup the webhook for a page.

                // Write back challenge in response
                //byte[] bytes = Encoding.UTF8.GetBytes(challenge);
                //Response.Body.Write(bytes, 0, bytes.Length);
                return Ok(challenge);
            }

            return Ok();
        }

        // POST facebook
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Callback([FromBody] string content)
        {
            JObject json = JObject.Parse(content);



            return Ok();
        }
    }
}
