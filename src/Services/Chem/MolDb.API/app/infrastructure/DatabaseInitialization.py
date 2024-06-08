from app.infrastructure.Database import GetDbPool


async def InitializeDb():
    """
    Sets up the database by creating necessary extensions and tables if they don't exist.

    Args:
        None

    Returns:
        None
    """
    db_pool = await GetDbPool()
    async with db_pool.acquire() as conn:
        try:
            # Enable RDKit extension if not already enabled
            await conn.execute(
                """
                DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_extension WHERE extname = 'rdkit') THEN
                        CREATE EXTENSION "rdkit";
                    END IF;
                END $$;
                """
            )
            # create uuid-ossp extension
            await conn.execute(
                """
                DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_extension WHERE extname = 'uuid-ossp') THEN
                        CREATE EXTENSION "uuid-ossp";
                    END IF;
                END $$;
                """
            )

            # Create the molecules table if it doesn't exist
            await conn.execute(
                """
                DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'molecules') THEN
                        CREATE TABLE molecules (
                            id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                            name VARCHAR(100),
                            smiles VARCHAR(500),
                            smiles_canonical VARCHAR(500),
                            molecular_weight DOUBLE PRECISION,
                            tpsa DOUBLE PRECISION,
                            mol MOL,
                            mfp2 BFP
                        );
                        CREATE INDEX mol_idx ON molecules USING gist (mol);
                        CREATE INDEX mfp2_idx ON molecules USING gist (mfp2);

                    END IF;
                END $$;
                """
            )
        finally:
            await conn.close()
