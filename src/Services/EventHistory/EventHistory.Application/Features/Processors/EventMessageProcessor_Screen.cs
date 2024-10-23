using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleScreenCreatedEventAsync(ScreenCreatedEvent screenCreatedEvent)
        {
            string primaryOrgName;
            string createdByUser = null;
            try
            {
                var primaryOrg = await _userStoreAPI.GetOrgById(screenCreatedEvent.PrimaryOrgId);
                primaryOrgName = primaryOrg?.Name ?? "Unknown";

                // Check if CreatedById is not null before calling GetUserById
                if (screenCreatedEvent.CreatedById.HasValue)
                {
                    var user = await _userStoreAPI.GetUserById(screenCreatedEvent.CreatedById.Value); // Use .Value to get the Guid
                    createdByUser = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    createdByUser = "Unknown User";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve primary organization or user details for ScreenCreatedEvent: {screenCreatedEvent.Id}");
                primaryOrgName = "Unknown";
            }
            return new EventMessageResult
            {
                Message = $"<b>{primaryOrgName}</b> added a new Screen <b>{screenCreatedEvent.Name}</b>, created by {createdByUser}",
                Link = $"/screen/{screenCreatedEvent.Id}"
            };
        }
    }
}