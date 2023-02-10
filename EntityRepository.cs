using Penguin.Cms.Entities;
using Penguin.Cms.Repositories.Interfaces;
using Penguin.Messaging.Core;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Persistence.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Cms.Repositories
{
    /// <summary>
    /// The base repository responsible for managing all CMS entities
    /// </summary>
    /// <typeparam name="T">Any CMS entity type</typeparam>
    public class EntityRepository<T> : KeyedObjectRepository<T>, IEntityRepository<T>, IEntityRepository where T : Entity
    {
        /// <summary>
        /// Constructs a new instance of this repository
        /// </summary>
        /// <param name="dbContext">Any persistence context for this entity</param>
        /// <param name="messageBus">An optional message bus to use for persistence messages</param>
        public EntityRepository(IPersistenceContext<T> dbContext, MessageBus messageBus = null) : base(dbContext, messageBus)
        {
        }

        /// <summary>
        /// Gets an entity based on its external id
        /// </summary>
        /// <param name="ExternalId">The external ID of the object to retrieve</param>
        /// <returns>An object with the matching ExternalID or null</returns>
        public virtual T Find(string ExternalId)
        {
            return this.Where(e => e.ExternalId == ExternalId).SingleOrDefault();
        }

        /// <summary>
        /// Gets an IEnumerable of objects from the Persistence Context that match the provided list. Useful for refreshing from the context
        /// </summary>
        /// <param name="o">The matching objects to return</param>
        /// <returns>The matching objects</returns>
        public override IEnumerable<T> Find(IEnumerable<T> o)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o), "Can not search for null object IEnumerable");
            }

            foreach (T to in o)
            {
                yield return to._Id != 0 ? Find(to._Id) : Find(to.Guid);
            }
        }

        /// <summary>
        /// Gets any individual object based on its Id (if saved) or its Guid (if not)
        /// </summary>
        /// <param name="o">the object to search for</param>
        /// <returns>The persistence context version of the object</returns>
        public T Find(T o)
        {
            return o is null ? throw new ArgumentNullException(nameof(o)) : o._Id == 0 ? Find(o.Guid) : Find(o._Id);
        }

        //Id might be 0 for static representations of entities. This should only be true for security groups
        //Which have a guid generated based on their external ID. If both properties are null, you're boned.
        /// <summary>
        /// Retrieves an object instance from the persistence context by its Guid
        /// </summary>
        /// <param name="guid">The Guid to look for</param>
        /// <returns>An object instance, or null</returns>
        public virtual T Find(Guid guid)
        {
            return this.FirstOrDefault(e => e.Guid == guid);
        }

        /// <summary>
        /// Retrieves an object instance from the persistence context by its Guid
        /// </summary>
        /// <param name="guid">The Guid to look for</param>
        /// <returns>An object instance, or null</returns>
        Entity IEntityRepository.Find(Guid guid)
        {
            return Find(guid);
        }

        /// <summary>
        /// Gets an entity based on its external id
        /// </summary>
        /// <param name="ExternalId">The external ID of the object to retrieve</param>
        /// <returns>An object with the matching ExternalID or null</returns>
        Entity IEntityRepository.Find(string ExternalId)
        {
            return Find(ExternalId);
        }

        /// <summary>
        /// Attempts to find the key type and passes it to the appropriate typed find method
        /// </summary>
        /// <param name="Key">The key to search for</param>
        /// <returns>An object with a key of the specified type that matches</returns>
        public override T Find(object Key)
        {
            if (Key is null)
            {
                throw new ArgumentNullException(nameof(Key));
            }

            if (Key is Guid g)
            {
                return Find(g);
            }
            else if (Key is string s)
            {
                return Find(s);
            }
            else if (Key is int i)
            {
                return Find(i);
            }
            else
            {
                Type t = GetType();

                while (t != typeof(object) && t != null && t.GetGenericArguments().Length != 1)
                {
                    t = t.BaseType;
                }

                if (t == typeof(object) || t == null)
                {
                    return base.Find(Key);
                }
                else
                {
                    //JIT Fucks up the generic sometimes apparently and returns __Canon, which causes
                    //the parameter to get treated as an object, breaking the repository if you dont redirect
                    //the call
                    return Find(Key as T);
                }
            }
        }

        /// <summary>
        /// Gets an IEnumerable of objects based on the Guid
        /// </summary>
        /// <param name="guids">The guids to search for</param>
        /// <returns>A list of entities where their ID was found in the provided list</returns>
        public virtual IEnumerable<T> FindRange(IEnumerable<Guid> guids)
        {
            return this.Where(e => guids.Contains(e.Guid));
        }

        /// <summary>
        /// Gets an IEnumerable of objects based on the External Ids
        /// </summary>
        /// <param name="ExternalIds">The External Ids to search for</param>
        /// <returns>A list of entities where their ID was found in the provided list</returns>
        public virtual IEnumerable<T> FindRange(IEnumerable<string> ExternalIds)
        {
            return this.Where(e => ExternalIds.Contains(e.ExternalId));
        }

        /// <summary>
        /// Gets an IEnumerable of objects based on the Guid
        /// </summary>
        /// <param name="guids">The guids to search for</param>
        /// <returns>A list of entities where their ID was found in the provided list</returns>
        IEnumerable<Entity> IEntityRepository.FindRange(IEnumerable<Guid> guids)
        {
            if (guids is null)
            {
                throw new ArgumentNullException(nameof(guids), "Can not search for null guids IEnumerable");
            }

            foreach (Guid g in guids)
            {
                Entity toReturn = Context.FirstOrDefault(e => e.Guid == g);

                if (toReturn != null)
                {
                    yield return toReturn;
                }
            }
        }

        /// <summary>
        /// Gets an IEnumerable of objects based on the External Ids
        /// </summary>
        /// <param name="ExternalIds">The External Ids to search for</param>
        /// <returns>A list of entities where their ID was found in the provided list</returns>
        IEnumerable<Entity> IEntityRepository.FindRange(IEnumerable<string> ExternalIds)
        {
            if (ExternalIds is null)
            {
                throw new ArgumentNullException(nameof(ExternalIds), "Can not search for null string IEnumerable");
            }

            foreach (string s in ExternalIds)
            {
                Entity toReturn = Context.FirstOrDefault(e => e.ExternalId == s);

                if (toReturn != null)
                {
                    yield return toReturn;
                }
            }
        }

        /// <summary>
        /// ShallowClones and object and resets its Guid/ExternalId/Id
        /// </summary>
        /// <param name="o">The object to clone</param>
        /// <returns>A clone of the object</returns>
        public override T ShallowClone(T o)
        {
            T newObject = base.ShallowClone(o);

            newObject.Guid = Guid.NewGuid();

            newObject.ExternalId = newObject.Guid.ToString();

            return newObject;
        }
    }
}