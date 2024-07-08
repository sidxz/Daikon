import logging
import os
import uvicorn
from fastapi import FastAPI
from app.api import QueryController, CommandController
from app.infrastructure.DatabaseInitialization import InitializeDb
from dotenv import load_dotenv

load_dotenv()
# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format="%(levelname)s - %(asctime)s - [%(name)s] - %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S",
)
logger = logging.getLogger(__name__)

# Initialize FastAPI app
app = FastAPI(title="Daikon Core Molecule.API", version="2.0")

@app.on_event("startup")
async def startup():
    """
    Perform startup activities
    - Database initialization
    """
    logger.info("Starting up the application")
    try:
        logger.info("Initializing database")
        await InitializeDb()
        logger.info("Database initialized successfully")
    except Exception as e:
        logger.error(f"Error setting up database: {e}", exc_info=True)
        raise
    


@app.on_event("shutdown")
async def shutdown():
    """
    Perform shutdown activities
    """
    logger.info("Shutting down the application")

# Include API routers
app.include_router(QueryController.router)
app.include_router(CommandController.router)

# Run the application
if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8101)
