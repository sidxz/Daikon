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
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ScreenCreatedEvent");
                throw;
            }
        }

        private async Task<EventMessageResult> HandleScreenUpdatedEventAsync(ScreenUpdatedEvent updatedEvent)
        {
            try
            {
                var organizationName = await GetOrganizationNameAsync(updatedEvent.PrimaryOrgId);
                var updatedByUser = updatedEvent.LastModifiedById.HasValue
                    ? await GetUserNameAsync(updatedEvent.LastModifiedById)
                    : "Unknown User";
                var link = GenerateLink(updatedEvent.ScreenType, updatedEvent.Id);

                return new EventMessageResult
                {
                    Message = $"Screen <b>{updatedEvent.Name}</b> (<b>{organizationName}</b>) was updated by {updatedByUser}",
                    Link = link,
                    EventType = nameof(ScreenUpdatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ScreenUpdatedEvent");
                throw;
            }
        }

        private async Task<EventMessageResult> HandleHitColCreatedEventAsync(HitCollectionCreatedEvent createdEvent)
        {
            string screenName = "Unknown Screen";
            string link = "#";
            string organizationName = "Unknown Organization";

            try
            {
                var screen = await _screenAPI.GetById(createdEvent.ScreenId);
                if (screen != null)
                {
                    screenName = screen.Name;
                    link = $"{GenerateLink(screen.ScreenType, screen.Id)}/hits/{createdEvent.Id}";
                    organizationName = await GetOrganizationNameAsync(screen.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning("Screen not found for ID: {ScreenId}", createdEvent.ScreenId);
                }

                var createdByUser = await GetUserNameAsync(createdEvent.CreatedById);

                return new EventMessageResult
                {
                    Message = $"<b>{organizationName}</b> added a new Hit Collection <b>{createdEvent.Name}</b> for {screenName}, created by {createdByUser}",
                    Link = link,
                    EventType = nameof(HitCollectionCreatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve data for HitCollectionCreatedEvent with Screen ID: {ScreenId}", createdEvent.ScreenId);
                throw;
            }
        }

        private async Task<EventMessageResult> HandleHitColUpdatedEventAsync(HitCollectionUpdatedEvent updatedEvent)
        {
            string screenName = "Unknown Screen";
            string link = "#";
            string organizationName = "Unknown Organization";

            try
            {
                var screen = await _screenAPI.GetById(updatedEvent.ScreenId);
                if (screen != null)
                {
                    screenName = screen.Name;
                    link = $"{GenerateLink(screen.ScreenType, screen.Id)}/hits/{updatedEvent.Id}";
                    organizationName = await GetOrganizationNameAsync(screen.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning("Screen not found for ID: {ScreenId}", updatedEvent.ScreenId);
                }

                var updatedByUser = updatedEvent.LastModifiedById.HasValue
                    ? await GetUserNameAsync(updatedEvent.LastModifiedById)
                    : "Unknown User";

                return new EventMessageResult
                {
                    Message = $"Hit Collection <b>{updatedEvent.Name}</b> for {screenName} (<b>{organizationName}</b>) was updated by {updatedByUser}",
                    Link = link,
                    EventType = nameof(HitCollectionUpdatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve data for HitCollectionUpdatedEvent with Screen ID: {ScreenId}", updatedEvent.ScreenId);
                throw;
            }
        }

        private async Task<EventMessageResult> HandleHitAddedEventAsync(HitAddedEvent addedEvent)
        {
            string screenName = "Unknown Screen";
            string hcName = "Unknown Hit Collection";
            string link = "#";
            string organizationName = "Unknown Organization";

            try
            {
                var hc = await _screenAPI.GetHitCollectionById(addedEvent.Id);
                if (hc != null)
                {
                    hcName = hc.Name;
                    var screen = await _screenAPI.GetById(hc.ScreenId);
                    if (screen != null)
                    {
                        screenName = screen.Name;
                        link = $"{GenerateLink(screen.ScreenType, screen.Id)}/hits/{hc.Id}";
                        organizationName = await GetOrganizationNameAsync(screen.PrimaryOrgId);
                    }
                    else
                    {
                        _logger.LogWarning("Screen not found for Hit Collection ID: {HitCollectionId}", hc.Id);
                    }
                }
                else
                {
                    _logger.LogWarning("Hit Collection not found for ID: {HitId}", addedEvent.Id);
                }

                var createdByUser = await GetUserNameAsync(addedEvent.CreatedById);

                return new EventMessageResult
                {
                    Message = $"<b>{organizationName}</b> added hits to <b>{hcName}</b> of {screenName}",
                    Link = link,
                    EventType = nameof(HitAddedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve data for HitAddedEvent with ID: {HitId}", addedEvent.Id);
                throw;
            }
        }

        // Helper method to generate link based on screen type
        private string GenerateLink(string screenType, Guid id)
        {
            var screenTypePath = screenType == "target-based" ? "/tb" : "/ph";
            return $"/wf/screen/viewer{screenTypePath}/{id}";
        }
    }
}
