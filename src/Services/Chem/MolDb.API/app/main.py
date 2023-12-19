import logging
import uvicorn
from fastapi import FastAPI
from app.API import QueryController, CommandController
from app.Infrastructure.DatabaseInitialization import InitializeDb

# Configure logging
logging.basicConfig(
    level=logging.INFO, format="%(levelname)s - %(asctime)s - [%(name)s] - %(message)s", datefmt="%Y-%m-%d %H:%M:%S"
)
logger = logging.getLogger(__name__)

# Initialize FastAPI app
app = FastAPI(title="Daikon Core Molecule.API", version="2.0")



@app.on_event("startup")
async def startup():
    """
    Perform startup activities, including database initialization.
    """
    logger.info("Starting up the application")
    try:
        await InitializeDb()
        logger.info("Database initialized successfully")
    except Exception as e:
        logger.error(f"Error setting up database: {e}", exc_info=True)
        raise


@app.on_event("shutdown")
async def shutdown():
    """
    Perform shutdown activities, such as closing database connections.
    """
    logger.info("Shutting down the application")
    # Close any resources if necessary


# Include API routers
app.include_router(QueryController.router)
app.include_router(CommandController.router)

# Run the application
if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
