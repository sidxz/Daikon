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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling HaCreatedEvent");
                throw;
            }
        }

        private async Task<EventMessageResult> HandleHaUpdatedEvent(HaUpdatedEvent haUpdatedEvent)
        {
            string haName = "Unknown Hit Assessment";
            string updatedByUser = "Unknown User";
            string organizationName = "Unknown Organization";

            try
            {
                var ha = await _hitAssessmentAPI.GetById(haUpdatedEvent.Id, this.refreshCache);
                if (ha != null)
                {
                    haName = ha.Name;
                    organizationName = await GetOrganizationNameAsync(ha.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning($"Hit Assessment not found: {haUpdatedEvent.Id}");
                }

                if (haUpdatedEvent.LastModifiedById != Guid.Empty)
                {
                    updatedByUser = await GetUserNameAsync(haUpdatedEvent.LastModifiedById);
                }

                return new EventMessageResult
                {
                    Message = $"Hit Assessment <b>{haName} ({organizationName})</b> was updated by {updatedByUser}",
                    Link = $"/wf/ha/viewer/{haUpdatedEvent.Id}",
                    EventType = nameof(HaUpdatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process HaUpdatedEvent for Id: {haUpdatedEvent.Id}");
                throw;
            }
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

        private async Task<EventMessageResult> HandleHACEAddedEvent(HaCompoundEvolutionAddedEvent haCompoundEvolutionEvent)
        {
            return await HandleCompoundEvolutionEvent(haCompoundEvolutionEvent, "A new Compound Evolution was added to the Hit Assessment");
        }

        private async Task<EventMessageResult> HandleHACEUpdatedEvent(HaCompoundEvolutionUpdatedEvent haCompoundEvolutionEvent)
        {
            return await HandleCompoundEvolutionEvent(haCompoundEvolutionEvent, "Compound Evolution of Hit Assessment was modified");
        }

        private async Task<EventMessageResult> HandleCompoundEvolutionEvent(dynamic haCompoundEvolutionEvent, string actionMessage)
        {
            string haName = "Unknown Hit Assessment";
            string updatedByUser = "Unknown User";
            string organizationName = "Unknown Organization";

            try
            {
                var ha = await _hitAssessmentAPI.GetById(haCompoundEvolutionEvent.Id, this.refreshCache);
                if (ha != null)
                {
                    haName = ha.Name;
                    organizationName = await GetOrganizationNameAsync(ha.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning($"Hit Assessment not found: {haCompoundEvolutionEvent.Id}");
                }

                if (haCompoundEvolutionEvent.LastModifiedById != Guid.Empty)
                {
                    updatedByUser = await GetUserNameAsync(haCompoundEvolutionEvent.LastModifiedById);
                }

                return new EventMessageResult
                {
                    Message = $"{actionMessage} <b>{haName} ({organizationName})</b> by {updatedByUser}",
                    Link = $"/wf/ha/viewer/{haCompoundEvolutionEvent.Id}",
                    EventType = haCompoundEvolutionEvent.GetType().Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process {haCompoundEvolutionEvent.GetType().Name} for Id: {haCompoundEvolutionEvent.Id}");
                throw;
            }
        }
    }
}
