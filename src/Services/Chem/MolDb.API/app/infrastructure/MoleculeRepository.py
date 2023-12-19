from app.Infrastructure.Database import GetDbPool
from app.DTO.Molecule import Molecule
from app.Core.Contracts.IMoleculeRepository import IMoleculeRepository
import logging


class MoleculeRepository(IMoleculeRepository):
    """
    Repository for handling molecule data operations.
    """

    def __init__(self):
        self.db_pool = None

    async def initialize(self):
        """
        Initialize the repository with a database pool.
        """
        if not self.db_pool:
            self.db_pool = await GetDbPool()

    async def add_molecule(
        self,
        name: str,
        smiles: str,
        smiles_canonical: str,
        molecular_weight: float,
        tpsa: float,
    ) -> dict:
        """
        Add a new molecule to the database.

        Args:
            name (str): Name of the molecule.
            smiles (str): SMILES representation of the molecule.
            smiles_canonical (str): Canonical SMILES representation.
            molecular_weight (float): Molecular weight of the molecule.
            tpsa (float): Topological Polar Surface Area.

        Returns:
            dict: A dictionary containing the details of the created molecule.
        """
        if not smiles:
            raise ValueError("SMILES string is required.")

        smiles = smiles.upper()

        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetchrow(
                    """
                    INSERT INTO molecules (name, smiles, smiles_canonical, mol, mfp2, molecular_weight, tpsa)
                    VALUES ($1, $2, $3, mol_from_smiles($4::cstring), morganbv_fp(mol_from_smiles($5::cstring)), $6, $7)
                    RETURNING id;
                    """,
                    name,
                    smiles,
                    smiles_canonical,
                    smiles,
                    smiles,
                    molecular_weight,
                    tpsa,
                )

                mol_id = result["id"]
                molecule = Molecule()
                return molecule.dump(
                    {
                        "id": mol_id,
                        "name": name,
                        "smiles": smiles,
                        "smilesCanonical": smiles_canonical,
                        "molecularWeight": molecular_weight,
                        "tpsa": tpsa,
                    }
                )

        except Exception as e:
            logging.error(f"Error adding molecule to the database: {e}")
            raise
