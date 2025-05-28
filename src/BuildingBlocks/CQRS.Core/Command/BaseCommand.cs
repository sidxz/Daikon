using System;
using CQRS.Core.Domain;

namespace CQRS.Core.Command
{
    /// <summary>
    /// Represents the base class for all commands, providing metadata for creation and modification tracking.
    /// </summary>
    public abstract class BaseCommand : DocMetadata
    {
        /// <summary>
        /// Gets or sets the unique identifier for the command.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who requested the command.
        /// </summary>
        public Guid RequestorUserId { get; set; }

        /// <summary>
        /// Sets the properties related to command creation, including timestamps and the requestor's ID.
        /// </summary>
        /// <param name="requestorUserId">The unique identifier of the user who is creating the command.</param>
        public void SetCreateProperties(Guid requestorUserId)
        {

            var currentTime = DateTime.UtcNow;
            DateCreated = currentTime;
            IsModified = false;
            CreatedById = requestorUserId;
            LastModifiedById = requestorUserId;
            DateModified = currentTime;
        }

        /// <summary>
        /// Sets the properties related to command modification, including timestamps and the requestor's ID.
        /// </summary>
        /// <param name="requestorUserId">The unique identifier of the user who is modifying the command.</param>
        public void SetUpdateProperties(Guid requestorUserId)
        {

            var currentTime = DateTime.UtcNow;
            IsModified = true;
            LastModifiedById = requestorUserId;
            DateModified = currentTime;
        }
    }
}
