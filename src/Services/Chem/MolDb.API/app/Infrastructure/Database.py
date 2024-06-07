import os
import logging
import asyncpg

# Configure logging
logging.basicConfig(level=logging.ERROR)
logger = logging.getLogger(__name__)


# Function to get environment variables or raise an error if not found
def get_env_variable(var_name):
    try:
        return os.environ[var_name]
    except KeyError:
        logger.error(f"Environment variable '{var_name}' not found.")
        raise EnvironmentError(f"Environment variable '{var_name}' not found.")


# Global variable to store the connection pool
db_pool = None


async def GetDbPool():
    global db_pool
    if db_pool is None:
        try:
            # Read environment variables
            USER = get_env_variable("MolDbUser")
            PASSWORD = get_env_variable("MolDbPassword")
            HOST = get_env_variable("MolDbHost")
            PORT = get_env_variable("MolDbPort")
            DATABASE = get_env_variable("MolDbDatabase")

            db_pool = await asyncpg.create_pool(
                user=USER, password=PASSWORD, host=HOST, port=PORT, database=DATABASE
            )
        except Exception as e:
            logger.error(f"Error creating database pool: {e}")
            raise
    return db_pool
