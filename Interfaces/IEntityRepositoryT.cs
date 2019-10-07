using Penguin.Cms.Entities;
using Penguin.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace Penguin.Cms.Repositories.Interfaces
{
    /// <summary>
    /// An interface representing the base repository for all entities that inherit from "Entity"
    /// </summary>
    /// <typeparam name="T">Any Entity Type</typeparam>
    public interface IEntityRepository<T> : IKeyedObjectRepository<T> where T : Entity
    {
        /// <summary>
        /// Gets an entity by its guid
        /// </summary>
        /// <param name="guid">The guid to get</param>
        /// <returns>An entity matching the Guid, or null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
        T Find(Guid guid);

        /// <summary>
        /// Gets an entity by its external ID
        /// </summary>
        /// <param name="ExternalId">The external Id of the entity to get</param>
        /// <returns>An entity with a matching external Id, or null</returns>
        T Find(string ExternalId);

        /// <summary>
        /// Gets an IEnumerable of entities where the Guid is found in the provided list
        /// </summary>
        /// <param name="guids">The list of Guids to search for</param>
        /// <returns>an IEnumerable of entities where the Guid is found in the provided list</returns>
        IEnumerable<T> FindRange(IEnumerable<Guid> guids);

        /// <summary>
        /// Gets an IEnumerable of entities where the External Id is found in the provided list
        /// </summary>
        /// <param name="ExternalIds">The list of External Ids to search for</param>
        /// <returns>an IEnumerable of entities where the External Id is found in the provided list</returns>
        IEnumerable<T> FindRange(IEnumerable<string> ExternalIds);
    }
}