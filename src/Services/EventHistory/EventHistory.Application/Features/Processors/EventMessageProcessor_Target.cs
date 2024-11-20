using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Targets;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleTargetCreatedEventAsync(TargetCreatedEvent createdEvent)
        {
            try
            {
                var createdByUser = await GetUserNameAsync(createdEvent.CreatedById);
                var link = $"/wf/target/viewer/{createdEvent.Id}";

                return new EventMessageResult
                {
                    Message = $"A new Target <b>{createdEvent.Name}</b> has been added by {createdByUser}",
                    Link = link,
                    EventType = nameof(TargetCreatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling TargetCreatedEvent");
                throw;
            }
        }

        private async Task<EventMessageResult> HandleTargetUpdatedEventAsync(TargetUpdatedEvent updatedEvent)
        {
            try
            {
                var updatedByUser = updatedEvent.LastModifiedById.HasValue
                    ? await GetUserNameAsync(updatedEvent.LastModifiedById)
                    : "Unknown User";
                var link = $"/wf/target/viewer/{updatedEvent.Id}";

                return new EventMessageResult
                {
                    Message = $"Target <b>{updatedEvent.Name}</b> was updated by {updatedByUser}",
                    Link = link,
                    EventType = nameof(TargetUpdatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling TargetUpdatedEvent");
                throw;
            }
        }
    }
}