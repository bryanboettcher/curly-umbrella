using System;
using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace HWI.Internal.Persistence.Raven
{
    public abstract class RavenDbRepository<T> : IDisposable where T : new()
    {
        private readonly IDocumentStore _documentStore;
        private readonly string _sessionName;
        
        protected RavenDbRepository(IDocumentStore documentStore, string sessionName)
        {
            _documentStore = documentStore;
            _sessionName = sessionName;
        }

        protected IDocumentSession GetDocumentSession() => _documentStore.OpenSession(_sessionName);
        
        public virtual void Create(T item)
        {
            using (var session = GetDocumentSession())
            {
                session.Store(item);
                session.SaveChanges();
            }
        }

        public virtual void Update(T item)
        {
            using (var session = GetDocumentSession())
            {
                session.Store(item);
                session.SaveChanges();
            }
        }

        public virtual IEnumerable<T> RetrieveAll()
        {
            var session = GetDocumentSession();
            return session.Query<T>();
        }

        public void Dispose()
        {
            _documentStore?.Dispose();
        }
    }
}