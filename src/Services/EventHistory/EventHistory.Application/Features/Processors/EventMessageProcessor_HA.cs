using System;
using System.Threading.Tasks;
using Daikon.Events.HitAssessment;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleHaCreatedEvent(HaCreatedEvent haCreatedEvent)
        {
            string primaryOrgName;
            string createdByUser = null;

            try
            {
                var primaryOrg = await _userStoreAPI.GetOrgById(haCreatedEvent.PrimaryOrgId);
                primaryOrgName = primaryOrg?.Name ?? "Unknown";

                // Check if CreatedById is not null before calling GetUserById
                if (haCreatedEvent.CreatedById.HasValue)
                {
                    var user = await _userStoreAPI.GetUserById(haCreatedEvent.CreatedById.Value); // Use .Value to get the Guid
                    createdByUser = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    createdByUser = "Unknown User";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve primary organization or user details for HaCreatedEvent: {haCreatedEvent.Id}");
                primaryOrgName = "Unknown";
                createdByUser = "Unknown User";
            }

            return new EventMessageResult
            {
                Message = $"<b>{primaryOrgName}</b> added a new Hit Assessment <b>{haCreatedEvent.Name}</b>, created by {createdByUser}",
                Link = $"/wf/ha/viewer/{haCreatedEvent.Id}"
            };
        }

        private async Task<EventMessageResult> HandleHaUpdatedEvent(HaUpdatedEvent haUpdatedEvent)
        {
            string haName;
            string updatedByUser = null;

            try
            {
                var ha = await _hitAssessmentAPI.GetById(haUpdatedEvent.Id);
                if (ha == null)
                {
                    _logger.LogWarning($"Hit Assessment not found: {haUpdatedEvent.Id}");
                    return new EventMessageResult
                    {
                        Message = $"Hit Assessment {haUpdatedEvent.Id} not found",
                        Link = string.Empty
                    };
                }
                haName = ha.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve Hit Assessment for Id: {haUpdatedEvent.Id}");
                haName = "Unknown";
            }

            var updatedById = haUpdatedEvent.LastModifiedById ?? Guid.Empty;

            if (updatedById != Guid.Empty)
            {
                try
                {
                    var user = await _userStoreAPI.GetUserById(updatedById);
                    var org = await _userStoreAPI.GetOrgById(user.AppOrgId);
                    if (user != null && org != null)
                    {
                        updatedByUser = $"{user.FirstName} {user.LastName}, {org.Alias}";
                    }
                    else
                    {
                        _logger.LogWarning($"User or Org not found for Id: {updatedById}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to retrieve user for Id: {updatedById}");
                }
            }

            return new EventMessageResult
            {
                Message = updatedByUser != null
                    ? $"The Hit Assessment <b>{haName}</b> was updated by {updatedByUser}"
                    : $"The Hit Assessment <b>{haName}</b> was updated.",
                Link = $"/wf/ha/viewer/{haUpdatedEvent.Id}"
            };
        }

        private EventMessageResult HandleHaDeletedEvent(HaDeletedEvent haDeletedEvent)
        {
            return new EventMessageResult
            {
                Message = $"Hit Assessment {haDeletedEvent.Id} was deleted",
                Link = $"/ha/{haDeletedEvent.Id}/deleted"
            };
        }
    }
}
