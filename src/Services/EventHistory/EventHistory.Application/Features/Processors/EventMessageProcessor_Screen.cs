using System;
using System.Threading.Tasks;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleScreenCreatedEventAsync(ScreenCreatedEvent createdEvent)
        {
            // Set default values
            var organizationName = await GetOrganizationNameAsync(createdEvent.PrimaryOrgId);
            var createdByUser = await GetUserNameAsync(createdEvent.CreatedById);

            var link = GenerateLink(createdEvent.ScreenType, createdEvent.Id);

            return new EventMessageResult
            {
                Message = $"<b>{organizationName}</b> added a new Screen <b>{createdEvent.Name}</b>, created by {createdByUser}",
                Link = link,
                EventType = nameof(ScreenCreatedEvent)
            };

        }


        private async Task<EventMessageResult> HandleScreenUpdatedEventAsync(ScreenUpdatedEvent updatedEvent)
        {
            // Set default values
            var organizationName = await GetOrganizationNameAsync(updatedEvent.PrimaryOrgId);
            string updatedByUser = "Unknown User";

            if (updatedEvent.LastModifiedById.HasValue)
            {
                updatedByUser = await GetUserNameAsync(updatedEvent.LastModifiedById);
            }


            var link = GenerateLink(updatedEvent.ScreenType, updatedEvent.Id);

            return new EventMessageResult
            {
                Message = $"Screen <b>{updatedEvent.Name}</b>, (<b>{organizationName}</b>) was updated by {updatedByUser}",
                Link = link,
                EventType = nameof(ScreenCreatedEvent)
            };

        }


        private async Task<EventMessageResult> HandleHitColCreatedEventAsync(HitCollectionCreatedEvent createdEvent)
        {
            // Set default values
            string screenName = "Unknown Screen";
            string createdByUser = "Unknown User";
            string link = "#";
            string organizationName = "Unknown Organization";

            try
            {
                var screen = await _screenAPI.GetById(createdEvent.ScreenId);
                if (screen != null)
                {
                    screenName = screen.Name;
                    link = GenerateLink(screen.ScreenType, screen.Id) + $"/hits/{createdEvent.Id}";
                    organizationName = await GetOrganizationNameAsync(screen.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning($"Screen not found: {createdEvent.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve Screen for Id: {createdEvent.ScreenId}");
                screenName = "Unknown";
            }


            createdByUser = await GetUserNameAsync(createdEvent.CreatedById);

            return new EventMessageResult
            {
                Message = $"<b>{organizationName}</b> added a new Hit Collection <b>{createdEvent.Name}</b> for {screenName}, created by {createdByUser}",
                Link = link,
                EventType = nameof(HitCollectionCreatedEvent)
            };

        }


        private async Task<EventMessageResult> HandleHitColUpdatedEventAsync(HitCollectionUpdatedEvent updatedEvent)
        {
            string screenName = "Unknown Screen";
            string updatedByUser = "Unknown User";
            string link = "#";
            string organizationName = "Unknown Organization";

            try
            {
                var screen = await _screenAPI.GetById(updatedEvent.ScreenId);
                if (screen != null)
                {
                    screenName = screen.Name;
                    link = GenerateLink(screen.ScreenType, screen.Id) + $"/hits/{updatedEvent.Id}";
                    organizationName = await GetOrganizationNameAsync(screen.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning($"Screen not found: {updatedEvent.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve Screen for Id: {updatedEvent.ScreenId}");
                screenName = "Unknown";
            }


            if (updatedEvent.LastModifiedById.HasValue)
            {
                updatedByUser = await GetUserNameAsync(updatedEvent.LastModifiedById);
            }

            return new EventMessageResult
            {
                Message = $"Hit Collection <b>{updatedEvent.Name}</b>, for {screenName} (<b>{organizationName}</b>) was updated by {updatedByUser}",
                Link = link,
                EventType = nameof(HitCollectionUpdatedEvent)
            };
        }



        // Helper method to generate the link based on screen type
        private string GenerateLink(string screenType, Guid id)
        {
            string screenTypePath = screenType == "target-based" ? "/tb" : "/ph";
            return $"/wf/screen/viewer{screenTypePath}/{id}";
        }
    }
}
