from fastapi import FastAPI
from app.api import query_handlers, command_handlers
from app.infrastructure.database_setup import setup_database


# Initialize FastAPI app
app = FastAPI()

# Database initialization
@app.on_event("startup")
async def startup():
    print("Starting up")
    try :
        await setup_database()
    except Exception as e:
        print(f"Error setting up database: {e}")
        raise
    

@app.on_event("shutdown")
async def shutdown():
    print("Shutting down")
    
# Include API routers
app.include_router(query_handlers.router)
app.include_router(command_handlers.router)

# Optional: Add more route registrations or middleware here
