from fastapi import FastAPI
from app.API import QueryController, CommandController
from app.Infrastructure.DatabaseInitialization import InitializeDb


# Initialize FastAPI app
app = FastAPI()

# Database initialization
@app.on_event("startup")
async def startup():
    print("Starting up")
    try :
        await InitializeDb()
    except Exception as e:
        print(f"Error setting up database: {e}")
        raise
    

@app.on_event("shutdown")
async def shutdown():
    print("Shutting down")
    
# Include API routers
app.include_router(QueryController.router)
app.include_router(CommandController.router)

# Optional: Add more route registrations or middleware here
