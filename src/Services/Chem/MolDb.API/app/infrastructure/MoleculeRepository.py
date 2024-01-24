from uuid import UUID
from app.Infrastructure.Database import GetDbPool
from app.DTO.Molecule import Molecule
from app.DTO.SimilarMolecule import SimilarMolecule
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

    async def read_molecule_by_smiles(self, smiles: str) -> dict:
        """
        Read a molecule from the database by its SMILES representation.

        Args:
            smiles (str): SMILES representation of the molecule.

        Returns:
            dict: A dictionary containing the details of the molecule.
        """
        if not smiles:
            raise ValueError("SMILES string is required.")

        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetchrow(
                    "SELECT id, name, smiles, smiles_canonical, molecular_weight, tpsa FROM molecules WHERE smiles = $1;",
                    smiles,
                )

                molecule = Molecule()
                return molecule.dump(
                    {
                        "id": result["id"],
                        "name": result["name"],
                        "smiles": result["smiles"],
                        "smilesCanonical": result["smiles_canonical"],
                        "molecularWeight": result["molecular_weight"],
                        "tpsa": result["tpsa"],
                    }
                )

        except Exception as e:
            logging.error(f"Error reading molecule from the database: {e}")
            raise

    async def read_molecule(self, id: UUID) -> dict:
        """
        Read a molecule from the database by its ID.

        Args:
            id (UUID): ID of the molecule.

        Returns:
            dict: A dictionary containing the details of the molecule.
        """
        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetchrow(
                    "SELECT id, name, smiles, smiles_canonical, molecular_weight, tpsa FROM molecules WHERE id = $1;",
                    id,
                )

                molecule = Molecule()
                return molecule.dump(
                    {
                        "id": result["id"],
                        "name": result["name"],
                        "smiles": result["smiles"],
                        "smilesCanonical": result["smiles_canonical"],
                        "molecularWeight": result["molecular_weight"],
                        "tpsa": result["tpsa"],
                    }
                )

        except Exception as e:
            logging.error(f"Error reading molecule from the database: {e}")
            raise

    async def update_molecule(
        self,
        id: UUID,
        name: str,
        smiles: str,
        smiles_canonical: str,
        molecular_weight: float,
        tpsa: float,
    ) -> dict:
        """
        Update a molecule in the database.

        Args:
            name (str): Name of the molecule.
            smiles (str): SMILES representation of the molecule.
            smiles_canonical (str): Canonical SMILES representation.
            molecular_weight (float): Molecular weight of the molecule.
            tpsa (float): Topological Polar Surface Area.

        Returns:
            dict: A dictionary containing the details of the updated molecule.
        """
        if not smiles:
            raise ValueError("SMILES string is required.")

        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetchrow(
                    """
                    UPDATE molecules
                    SET smiles = $1, smiles_canonical = $2, mol = mol_from_smiles($3::cstring), mfp2 = morganbv_fp(mol_from_smiles($4::cstring)), molecular_weight = $5, tpsa = $6, name = $7
                    WHERE id = $8
                    RETURNING id;
                    """,
                    smiles,
                    smiles_canonical,
                    smiles,
                    smiles,
                    molecular_weight,
                    tpsa,
                    name,
                    id,
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
            logging.error(f"Error updating molecule in the database: {e}")
            raise

    async def delete_molecule(self, id: UUID) -> dict:
        """
        Delete a molecule from the database.

        Args:
            name (str): Name of the molecule.

        Returns:
            dict: A dictionary containing the details of the deleted molecule.
        """
        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetchrow(
                    "DELETE FROM molecules WHERE id = $1 RETURNING id;", id
                )

                mol_id = result["id"]
                molecule = Molecule()
                return molecule.dump({"id": mol_id})

        except Exception as e:
            logging.error(f"Error deleting molecule from the database: {e}")
            raise

    async def find_similar_molecule(
        self, smiles: str, threshold: float = 0.8, num_results: int = 10
    ) -> list:
        """
        Find a similar molecule in the database.

        Args:
            smiles (str): SMILES representation of the molecule.
            threshold (float, optional): Similarity threshold. Defaults to 0.8.
            num_results (int, optional): Number of results to return. Defaults to 10.

        Returns:
            List of dict: A list of dictionaries containing the details of the similar molecule and their similarity scores.

        Purpose: 
            The query identifies molecules from the 'molecules' table that are structurally similar 
            to a query molecule, based on Tanimoto similarity.

        Process:
            Fingerprint Generation: For both the query molecule and each molecule in the database, 
                Morgan fingerprints (specifically, binary vector fingerprints with a radius of 2, denoted as mfp2) are generated.
                
            Similarity Calculation: The Tanimoto similarity between the query molecule's fingerprint 
                and each database molecule's fingerprint is calculated.
            
        """
        if not smiles:
            raise ValueError("SMILES string is required.")

        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetch(
                    """
                    SELECT id, name, smiles, smiles_canonical, molecular_weight, tpsa,
                        tanimoto_sml(morganbv_fp(mol_from_smiles($1::cstring)), mfp2) AS similarity
                    FROM molecules 
                    WHERE tanimoto_sml(morganbv_fp(mol_from_smiles($1::cstring)), mfp2) >= $2
                    ORDER BY similarity DESC
                    LIMIT $3;
                    """,
                    smiles,
                    threshold,
                    num_results,
                )

                molecule = SimilarMolecule()
                return [
                    molecule.dump(
                        {
                            "id": mol["id"],
                            "name": mol["name"],
                            "smiles": mol["smiles"],
                            "smilesCanonical": mol["smiles_canonical"],
                            "molecularWeight": mol["molecular_weight"],
                            "tpsa": mol["tpsa"],
                            "similarity": round(mol["similarity"], 2),
                        }
                    )
                    for mol in result
                ]

        except Exception as e:
            logging.error(f"Error finding similar molecule in the database: {e}")
            raise

    async def list_molecules(self) -> dict:
        """
        List all molecules in the database.

        Returns:
            dict: A dictionary containing the details of the molecules.
        """
        try:
            async with self.db_pool.acquire() as conn:
                result = await conn.fetch(
                    """
                    SELECT id, name, smiles, smiles_canonical, molecular_weight, tpsa
                    FROM molecules;
                    """
                )

                molecule = Molecule()
                return [
                    molecule.dump(
                        {
                            "id": mol["id"],
                            "name": mol["name"],
                            "smiles": mol["smiles"],
                            "smilesCanonical": mol["smiles_canonical"],
                            "molecularWeight": mol["molecular_weight"],
                            "tpsa": mol["tpsa"],
                        }
                    )
                    for mol in result
                ]

        except Exception as e:
            logging.error(f"Error listing molecules in the database: {e}")
            raise
