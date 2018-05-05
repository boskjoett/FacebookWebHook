using Microsoft.AspNetCore.Mvc;
using FacebookWebHook.Repository;
using System;
using System.Net.Http;

namespace FacebookWebHook.Controllers
{
    /// <summary>
    /// Facebook OAuth login callback handler
    /// </summary>
    [Route("facebook/login")]
    public class LoginController : Controller
    {
        private readonly IRepository repository;

        public LoginController(IRepository repository)
        {
            this.repository = repository;
        }


        /// <summary>
        /// Handler for Facebook OAuth GET requests triggered by a facebook Login request.
        /// </summary>
        [HttpGet]
        public IActionResult Get([FromQuery(Name = "code")] string code)
        {
            repository.Add(new RepositoryItem { Created = DateTime.Now, Message = $"QueryString: {Request.QueryString}" });

            if (Request.Query.ContainsKey("error"))
            {
                repository.Add(new RepositoryItem { Created = DateTime.Now, Message = $"Login failed. Reason: {Request.Query["error_description"]}" });
            }
            else
            {
/*
                // Exchange code with token
                using (HttpClient client = new HttpClient())
                {
                    string AppID = "209623639792426";
                    string AppSecret = "209623639792426";
                    string redirect_uri = "dfjsdfgjhfg";
                    
                    using (HttpResponseMessage res = client.GetAsync($"https://www.facebook.com/v2.12/oauth/access_token?clientid={AppID}&code={code}&client_secret={AppSecret}&redirect_uri={redirect_uri}").Result)
                    {
                        using (HttpContent content = res.Content)
                        {
                            string data = content.ReadAsStringAsync().Result;
                            if (data != null)
                            {
                                Console.WriteLine(data);
                            }
                        }
                    }
                }
*/
            }

            return Ok();
        }
    }
}
