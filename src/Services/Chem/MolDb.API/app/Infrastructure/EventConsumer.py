from confluent_kafka import Consumer, KafkaError
import asyncio
import json
import logging
import threading
from app.config.KafkaConfig import KAFKA_CONFIG
from app.infrastructure.EventHandler import EventHandler

logger = logging.getLogger(__name__)
event_handler = EventHandler();
class EventConsumer:
    def __init__(self, topics):
        

        # Check if KAFKA_BOOTSTRAP_SERVERS and group id are set, if not raise
        # an exception
        if not KAFKA_CONFIG["bootstrap.servers"]:
            raise Exception("KAFKA_BOOTSTRAP_SERVERS is not set")
        if not KAFKA_CONFIG["group.id"]:
            raise Exception("KAFKA_GROUP_ID is not set")

        self.consumer = Consumer(KAFKA_CONFIG)
        self.topics = topics
        

    def start(self):
        self.thread = threading.Thread(target=self.run, daemon=True)
        self.thread.start()

    def run(self):
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        loop.run_until_complete(self.consume())
        loop.close()

    async def consume(self):
        self.consumer.subscribe(self.topics)

        while True:
            message = self.consumer.poll(1.0)
            if message is None:
                await asyncio.sleep(1)
                continue
            if message.error():
                if message.error().code() != KafkaError._PARTITION_EOF:
                    logger.error(f"Kafka error: {message.error()}")
                continue

            await self.process_message(message)

    async def process_message(self, message):
        try:
            event_data = json.loads(message.value().decode("utf-8"))
            logger.info(f"Received event: {event_data}")
            logger.info(f"Event type: {event_data.get('Type')}")
            logger.info(f"Will call event handler")
            
            await event_handler.handle(event_data)
        except json.JSONDecodeError as e:
            logger.error(f"Error decoding message: {e}")

    async def stop(self):
        self.consumer.close()
        self.thread.join()
