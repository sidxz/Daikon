using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace CQRS.Core.Extensions
{

    public class VMUpdateTracker
    {
        /// <summary>
        /// Calculates the last updated date and the corresponding user ID 
        /// for a collection of trackable entities.
        /// </summary>
        /// <param name="trackableEntities">A collection of entities with metadata including modification date and user ID.</param>
        /// <returns>
        /// A tuple containing the latest modification date and the associated user ID. 
        /// If no updates are found, the default date is DateTime.MinValue and the user ID is Guid.Empty.
        /// </returns>
        public static (DateTime lastUpdatedDate, Guid lastUpdatedUserId) CalculatePageLastUpdated(IEnumerable<VMMeta> trackableEntities)
        {
            // Validate input to ensure it's not null.
            if (trackableEntities == null || !trackableEntities.Any())
            {
                return (DateTime.MinValue, Guid.Empty);
            }

            // Find the latest modified entity, treating null DateModified as DateTime.MinValue
            var latestEntity = trackableEntities
                                .OrderByDescending(e => e.DateModified ?? DateTime.MinValue) // Set null dates to DateTime.MinValue
                                .FirstOrDefault();

            if (latestEntity != null)
            {
                // If found, return the date (using DateTime.MinValue if null) and user ID, handling nullable LastModifiedById.
                var lastModifiedDate = latestEntity.DateModified ?? DateTime.MinValue;
                var lastModifiedUserId = latestEntity.LastModifiedById ?? Guid.Empty;
                return (lastModifiedDate, lastModifiedUserId);
            }

            // If no entities are valid, return default values.
            return (DateTime.MinValue, Guid.Empty);
        }
    }
}
