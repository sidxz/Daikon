using System;
using System.Threading.Tasks;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleScreenCreatedEventAsync(ScreenCreatedEvent screenCreatedEvent)
        {
            // Set default values
            var organizationName = await GetOrganizationNameAsync(screenCreatedEvent.PrimaryOrgId);
            var createdByUser = await GetUserNameAsync(screenCreatedEvent.CreatedById);

            var link = GenerateLink(screenCreatedEvent);

            return new EventMessageResult
            {
                Message = $"<b>{organizationName}</b> added a new Screen <b>{screenCreatedEvent.Name}</b>, created by {createdByUser}",
                Link = link
            };

        }

        // Helper method to generate the link based on screen type
        private string GenerateLink(ScreenCreatedEvent screenCreatedEvent)
        {
            string screenTypePath = screenCreatedEvent.ScreenType == "target-based" ? "/tb" : "/ph";
            return $"/wf/screen/viewer{screenTypePath}/{screenCreatedEvent.Id}";
        }
    }
}
