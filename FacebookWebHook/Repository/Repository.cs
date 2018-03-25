using System;
using System.Collections.Generic;

namespace FacebookWebHook.Repository
{
    public class Repository : IRepository
    {
        private readonly Dictionary<int, RepositoryItem> repository;

        public event EventHandler<RepositoryItem> ItemAdded;

        public Repository()
        {
            repository = new Dictionary<int, RepositoryItem>();

            Seed();
        }

        public IEnumerable<RepositoryItem> GetAll()
        {
            return repository.Values;
        }

        public RepositoryItem Get(int id)
        {
            return repository[id];
        }

        public void Add(RepositoryItem item)
        {
            repository.Add(repository.Count + 1, item);

            ItemAdded?.Invoke(this, item);
        }

        public void Update(int id, RepositoryItem item)
        {
            repository[id] = item;
        }

        public void Remove(int id)
        {
            repository.Remove(id);
        }

        private void Seed()
        {
            Add(new RepositoryItem { Created = new DateTime(2017, 7, 27), Message = "Happy birthday" });
            Add(new RepositoryItem { Created = DateTime.Now, Message = "Message of the day" });
        }
    }
}
