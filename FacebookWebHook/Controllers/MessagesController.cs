using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FacebookWebHook.Repository;

namespace FacebookWebHook.Controllers
{
    [Produces("application/json")]
    [Route("api/Messages")]
    public class MessagesController : Controller
    {
        private readonly IRepository repository;

        public MessagesController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/Messages
        [HttpGet]
        public IEnumerable<RepositoryItem> Get()
        {
            return repository.GetAll();
        }

        // GET: api/Messages/5
        [HttpGet("{id}", Name = "Get")]
        public RepositoryItem Get(int id)
        {
            return repository.Get(id);
        }
        
        // POST: api/Messages
        [HttpPost]
        public void Post(RepositoryItem item)
        {
            repository.Add(item);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public void Put(int id, RepositoryItem item)
        {
            repository.Update(id, item);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            repository.Remove(id);
        }
    }
}
