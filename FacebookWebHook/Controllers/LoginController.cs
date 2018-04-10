using Microsoft.AspNetCore.Mvc;
using FacebookWebHook.Repository;
using System;

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
            repository.Add(new RepositoryItem { Created = DateTime.Now, Message = $"code: {code}" });

            if (Request.Query.ContainsKey("error"))
            {
                repository.Add(new RepositoryItem { Created = DateTime.Now, Message = $"Login failed. Reason: {Request.Query["error_description"]}" });
            }

            return Ok();
        }
    }
}
