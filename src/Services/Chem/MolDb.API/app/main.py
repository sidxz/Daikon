import logging
import os
import uvicorn
from fastapi import FastAPI
from app.API import QueryController, CommandController
from app.Infrastructure.DatabaseInitialization import InitializeDb
from app.Infrastructure.EventConsumer import EventConsumer

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format="%(levelname)s - %(asctime)s - [%(name)s] - %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S",
)
logger = logging.getLogger(__name__)


# # Initialize Kafka consumer
# # Get the topics from environment variable
# topics = os.getenv("KafkaConsumerSettings_Topics").split(",")
# logger.info(f"Kafka topic: {topics}")
# if not topics[0]:
#     logger.error("KAFKA_TOPIC is not set")
#     raise Exception("KAFKA_TOPIC is not set")

# logger.info("Initializing Kafka consumer")
# consumer = EventConsumer(topics)


# Initialize FastAPI app
app = FastAPI(title="Daikon Core Molecule.API", version="2.0")


@app.on_event("startup")
async def startup():
    """
    Perform startup activities
    - Database initialization
    - Kafka consumer initialization
    """
    logger.info("Starting up the application")
    try:
        logger.info("Initializing database")
        await InitializeDb()
        logger.info("Database initialized successfully")
    except Exception as e:
        logger.error(f"Error setting up database: {e}", exc_info=True)
        raise
    # try:
    #     logger.info("Starting Kafka consumer")
    #     consumer.start()
    #     logger.info("Kafka consumer started successfully")
    # except Exception as e:
    #     logger.error(f"Error starting Kafka consumer: {e}", exc_info=True)
    #     raise


@app.on_event("shutdown")
async def shutdown():
    """
    Perform shutdown activities
    - Shutdown Kafka consumer
    """
    logger.info("Shutting down the application")
    # try:
    #     logger.info("Shutting down Kafka consumer")
    #     consumer.stop()
    #     logger.info("Kafka consumer shut down successfully")
    # except Exception as e:
    #     logger.error(f"Error shutting down Kafka consumer: {e}", exc_info=True)
    #     raise

# Include API routers
app.include_router(QueryController.router)
app.include_router(CommandController.router)

# Run the application
if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
