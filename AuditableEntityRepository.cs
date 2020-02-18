using Penguin.Cms.Entities;
using Penguin.Messaging.Abstractions.Interfaces;
using Penguin.Messaging.Core;
using Penguin.Messaging.Persistence.Messages;
using Penguin.Persistence.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Penguin.Cms.Repositories
{
    /// <summary>
    /// The base repository for entites that should have changes tracked and logged
    /// </summary>
    /// <typeparam name="T">Any type inheriting from AuditableEntity</typeparam>
    public class AuditableEntityRepository<T> : EntityRepository<T>, IMessageHandler<Deleting<T>>, IMessageHandler<Updating<T>>, IMessageHandler<Creating<T>> where T : AuditableEntity
    {
        /// <summary>
        /// An override to access all objects, does not return objects that have been deleted
        /// </summary>
        public override IQueryable<T> All => base.All.Where(e => e.DateDeleted == null);

        /// <summary>
        /// Creates a new instance of the auditable entity repository
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="messageBus"></param>
        public AuditableEntityRepository(IPersistenceContext<T> dbContext, MessageBus messageBus = null) : base(dbContext, messageBus)
        {
        }

        /// <summary>
        /// A message handler for "Created" events to set the date created
        /// </summary>
        /// <param name="createMessage">The object message containing the object</param>
        public virtual void AcceptMessage(Creating<T> createMessage)
        {
            Contract.Requires(createMessage != null);

            createMessage.Target.DateCreated = DateTime.Now;
        }

        /// <summary>
        /// A message handler for "Deleting" events to set the date deleted
        /// </summary>
        /// <param name="deleteMessage">The object message containing the object</param>
        public virtual void AcceptMessage(Deleting<T> deleteMessage)
        {
            Contract.Requires(deleteMessage != null);

            deleteMessage.Target.DateDeleted = DateTime.Now;
            deleteMessage.Target.DateModified = DateTime.Now;
        }

        /// <summary>
        /// A message handler for the "Update" event that sets the modified property
        /// </summary>
        /// <param name="updateMessage"></param>
        public virtual void AcceptMessage(Updating<T> updateMessage)
        {
            Contract.Requires(updateMessage != null);

            updateMessage.Target.DateModified = DateTime.Now;
        }

        public void Delete(T o, bool Force)
        {
            if (Force)
            {
                base.Delete(o);
            }
            else
            {
                o.DateDeleted = DateTime.Now;
            }
        }

        public override void Delete(T o)
        {
            o.DateDeleted = DateTime.Now;
        }

        public void DeleteRange(IEnumerable<T> o, bool Force)
        {
            if (Force)
            {
                base.DeleteRange(o);
            }
            else
            {
                foreach (T i in o)
                {
                    base.DeleteRange(o);
                }
            }
        }

        public override void DeleteRange(IEnumerable<T> o)
        {
            foreach (T i in o)
            {
                base.DeleteRange(o);
            }
        }
    }
}