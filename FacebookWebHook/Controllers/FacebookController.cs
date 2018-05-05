using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using FacebookWebHook.Repository;

namespace FacebookWebHook.Controllers
{
    /// <summary>
    /// Facebook Callback URL handler.
    /// In the App Dashboard at https://developers.facebook.com add a webhook subscription for 
    /// the Facebook page and set the callback URL to https://[hostname]/webhook/facebook
    /// See: https://developers.facebook.com/docs/graph-api/webhooks
    /// 
    /// URL: https://facebookwebhooktest.azurewebsites.net/webhook/facebook?hub.mode=subscribe&hub.challenge=ABCDEF123456&hub.verify_token=myfacebooktoken
    /// </summary>

    [Route("webhook/facebook")]
    public class FacebookController : Controller
    {
        private readonly IRepository repository;

        public FacebookController(IRepository repository)
        {
            this.repository = repository;
        }

        // Action handler for GET requests from Facebook
        [HttpGet]
        public IActionResult Get(
            [FromQuery(Name="hub.mode")] string mode,
            [FromQuery(Name="hub.challenge")] string challenge,
            [FromQuery(Name="hub.verify_token")] string verifyToken)
        {
            repository.Add(new RepositoryItem { Created = DateTime.Now, Message = "Challenge request from Facebook" });

            if (string.IsNullOrEmpty(mode) ||
                string.IsNullOrEmpty(challenge) ||
                string.IsNullOrEmpty(verifyToken) ||
                mode != "subscribe")
            {
                return Ok();
            }

            if (verifyToken == "***zylinc***")
            {
                // The verify token is defined in the Facebook App Dashboard where you setup the webhook for a page.

                repository.Add(new RepositoryItem { Created = DateTime.Now, Message = $"Challenge = {challenge}" });

                // Write back challenge in response
                //byte[] bytes = Encoding.UTF8.GetBytes(challenge);
                //Response.Body.Write(bytes, 0, bytes.Length);
                return Ok(challenge);
            }

            return Ok();
        }

        // Action handler for POST requests from Facebook
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Callback()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                repository.Add(new RepositoryItem { Created = DateTime.Now, Message = "Webhook callback from Facebook" });

                string jsonText = reader.ReadToEnd();
                repository.Add(new RepositoryItem { Created = DateTime.Now, Message = jsonText });
/*
                try
                {
                    JObject json = JObject.Parse(jsonText);

                    dynamic json2 = JObject.Parse(jsonText);

                    int n = json2.Age;
                    string txt = json2.Line;
                }
                catch (Exception)
                {
                }
*/
            }

            return Ok();
        }
    }
}
