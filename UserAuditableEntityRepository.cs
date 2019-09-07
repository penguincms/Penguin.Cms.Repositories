using Penguin.Cms.Entities;
using Penguin.Messaging.Core;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Persistence.Repositories;

namespace Penguin.Cms.Repositories
{
    /// <summary>
    /// This class should accept a user session and update the creator, but it does not currently.
    /// </summary>
    /// <typeparam name="T">Any user auditable entity type</typeparam>
    public class UserAuditableEntityRepository<T> : AuditableEntityRepository<T> where T : UserAuditableEntity
    {
        /// <summary>
        /// Creates a new instance of this repository
        /// </summary>
        /// <param name="dbContext">The persistence context to use for the obejcts</param>
        /// <param name="messageBus">And optional message bus for persistence messages</param>
        public UserAuditableEntityRepository(IPersistenceContext<T> dbContext, MessageBus messageBus = null) : base(dbContext, messageBus)
        {
        }
    }
}