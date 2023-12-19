import asyncpg

#DATABASE_URL = "postgresql://user:password@MolDb_Postgres:5432/chemdb"
USER="user"
PASSWORD="password"
HOST="MolDb_Postgres"
PORT="5432"
DATABASE="chemdb"


async def GetDbPool():
    try:
        return await asyncpg.create_pool(user=USER, password=PASSWORD, host=HOST, port=PORT, database=DATABASE)
    except Exception as e:
        print(f"Error creating database pool: {e}")
        raise