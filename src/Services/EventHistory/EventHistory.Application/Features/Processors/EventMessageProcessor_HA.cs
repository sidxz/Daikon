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
            var organizationName = await GetOrganizationNameAsync(haCreatedEvent.PrimaryOrgId);
            var createdByUser = await GetUserNameAsync(haCreatedEvent.CreatedById);

            return new EventMessageResult
            {
                Message = $"<b>{organizationName}</b> added a new Hit Assessment <b>{haCreatedEvent.Name}</b>, created by {createdByUser}",
                Link = $"/wf/ha/viewer/{haCreatedEvent.Id}",
                EventType = nameof(HaCreatedEvent)
            };
        }

        private async Task<EventMessageResult> HandleHaUpdatedEvent(HaUpdatedEvent haUpdatedEvent)
        {
            string haName;
            string updatedByUser = "Unknown User";

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

            // Fetch the user who last modified the assessment if their ID is provided
            if (haUpdatedEvent.LastModifiedById.HasValue)
            {
                updatedByUser = await GetUserNameAsync(haUpdatedEvent.LastModifiedById);
            }

            return new EventMessageResult
            {
                Message = updatedByUser != null
                    ? $"The Hit Assessment <b>{haName}</b> was updated by {updatedByUser}"
                    : $"The Hit Assessment <b>{haName}</b> was updated.",
                Link = $"/wf/ha/viewer/{haUpdatedEvent.Id}",
                EventType = nameof(HaUpdatedEvent)
            };
        }

        private EventMessageResult HandleHaDeletedEvent(HaDeletedEvent haDeletedEvent)
        {
            return new EventMessageResult
            {
                Message = $"Hit Assessment {haDeletedEvent.Id} was deleted",
                Link = $"/ha/{haDeletedEvent.Id}/deleted",
                EventType = nameof(HaDeletedEvent)
            };
        }
    }
}
