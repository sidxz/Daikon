using System;
using CQRS.Core.Event;
using Daikon.Events.HitAssessment;

namespace EventHistory.Application.Features.Processors
{
    public class EventToMessage
    {
        /// <summary>
        /// Processes the event data and returns an object containing both the message and the link.
        /// </summary>
        /// <param name="eventData">Event data object</param>
        /// <returns>EventMessageResult containing both message and link</returns>
        public static EventMessageResult Process(BaseEvent eventData)
        {
            if (eventData is HaCreatedEvent haCreatedEvent)
            {
                return new EventMessageResult
                {
                    Message = $"HA {haCreatedEvent.Name} created by {haCreatedEvent.PrimaryOrgId}",
                    Link = $"/ha/{haCreatedEvent.Id}"
                };
            }
            else if (eventData is HaUpdatedEvent haUpdatedEvent)
            {
                return new EventMessageResult
                {
                    Message = $"HA {haUpdatedEvent.Id} updated",
                    Link = $"/ha/{haUpdatedEvent.Id}/updated"
                };
            }
            else if (eventData is HaDeletedEvent haDeletedEvent)
            {
                return new EventMessageResult
                {
                    Message = $"HA {haDeletedEvent.Id} deleted",
                    Link = $"/ha/{haDeletedEvent.Id}/deleted"
                };
            }

            // Return a default response if the event is unsupported
            return new EventMessageResult
            {
                Message = "Unsupported event",
                Link = $"/ha/unsupported"
            };
        }
    }

    /// <summary>
    /// Class to encapsulate both message and link returned from event processing.
    /// </summary>
    public class EventMessageResult
    {
        public string Message { get; set; } = "";
        public string Link { get; set; } = "";
    }
}
