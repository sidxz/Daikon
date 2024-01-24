# Handle Events received in KAFKA
import logging
from app.Core.EventHandlers.Screens.HitAddedEventHandler import HandleHitAddedEvent
logger = logging.getLogger(__name__)

class EventHandler:
    def __init__(self):
        # Map event types to handler methods
        self.event_handlers = {
            "HitAddedEvent": HandleHitAddedEvent,
            # Add more mappings for other event types
        }

    async def handle(self, event_data):
        try:
            logger.info(f"Handling event: {event_data}")
            # Get the event type from the event data
            event_type = event_data.get("Type")

            # If the event type is not present, log a warning and return
            if not event_type:
                logger.error(f"Event type not found in event data: {event_data}")
                return

            # Get the handler method from the dictionary
            handler = self.event_handlers.get(event_type)

            if handler:
                logger.info(f"Found handler for event type: {event_type}")
                # Call the handler method
                
                await handler(event_data)
            else:
                logger.info(f"No registered handler found for event type: {event_type}. Acknowledging and skipping.")
        except Exception as e:
            logger.error(f"An error occurred while handling the event: {e}")
