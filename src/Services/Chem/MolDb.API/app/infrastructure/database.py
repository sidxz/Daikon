import asyncpg

#DATABASE_URL = "postgresql://user:password@MolDb_Postgres:5432/chemdb"
USER="user"
PASSWORD="password"
HOST="MolDb_Postgres"
PORT="5432"
DATABASE="chemdb"

async def get_db():
    try:
        conn = await asyncpg.connect(user=USER, password=PASSWORD, host=HOST, port=PORT, database=DATABASE)
        return conn
    except Exception as e:
        print(f"Error connecting to the database: {e}")
