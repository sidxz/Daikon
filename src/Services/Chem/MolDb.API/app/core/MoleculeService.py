from uuid import UUID
from rdkit import Chem
import logging
from app.core.MoleculeServices.MolecularProperties import CalculateMolecularProperties
from app.core.MoleculeServices.SmilesValidation import ValidateSmiles
from app.core.Contracts.IMoleculeRepository import IMoleculeRepository


class MoleculeService:
    def __init__(self, molecule_repository: IMoleculeRepository):
        self.molecule_repository = molecule_repository
        self.logger = logging.getLogger(__name__)

    async def createMolecule(self, name: str, smiles: str):
        """
        Create a molecule with the given name and SMILES representation.

        Args:
            name (str): The name of the molecule.
            smiles (str): The SMILES representation of the molecule.

        Returns:
            dict: A dictionary containing the details of the created molecule.
        """
        # Validate molecule data
        try:
            await ValidateSmiles(smiles)
        except ValueError as e:
            self.logger.error(f"Invalid SMILES string: {e}", exc_info=True)
            raise


        # Calculate Canonical SMILES
        smiles_canonical = Chem.MolToSmiles(Chem.MolFromSmiles(smiles), True)

        # Calculate molecular weight and Topological polar surface area (TPSA)
        molecular_properties = CalculateMolecularProperties(smiles)

        # Add molecule to database
        return await self.molecule_repository.add_molecule(
            name,
            smiles,
            smiles_canonical,
            molecular_properties["molecular_weight"],
            molecular_properties["tpsa"],
        )

    async def readMoleculeBySmile(self, smile: str):
        """
        Read a molecule from the database using the SMILES representation.

        Args:
            smile (str): The SMILES representation of the molecule.

        Returns:
            dict: A dictionary containing the details of the molecule.
        """
        return await self.molecule_repository.read_molecule_by_smiles(smile)

    async def updateMolecule(self, id: UUID, name: str, smiles: str):
        """
        Update a molecule with the given ID, name, and SMILES representation.

        Args:
            id (UUID): The ID of the molecule to update.
            name (str): The new name of the molecule.
            smiles (str): The new SMILES representation of the molecule.

        Returns:
            dict: A dictionary containing the details of the updated molecule.
        """
        # Validate molecule data
        try:
            await ValidateSmiles(smiles)
        except ValueError as e:
            self.logger.error(f"Invalid SMILES string: {e}", exc_info=True)
            raise


        # Calculate Canonical SMILES
        smiles_canonical = Chem.MolToSmiles(Chem.MolFromSmiles(smiles), True)

        # Calculate molecular weight and Topological polar surface area (TPSA)
        molecular_properties = CalculateMolecularProperties(smiles)

        # Update molecule in database
        return await self.molecule_repository.update_molecule(
            id,
            name,
            smiles,
            smiles_canonical,
            molecular_properties["molecular_weight"],
            molecular_properties["tpsa"],
        )

    async def deleteMolecule(self, id: UUID):
        """
        Delete a molecule with the given ID.

        Args:
            id (UUID): The ID of the molecule to delete.
        """
        return await self.molecule_repository.delete_molecule(id)

    async def readMolecule(self, id: UUID):
        """
        Read a molecule from the database using the ID.

        Args:
            id (UUID): The ID of the molecule.

        Returns:
            dict: A dictionary containing the details of the molecule.
        """
        return await self.molecule_repository.read_molecule(id)

    async def findExactMatch(self, smiles: str):
        """
        Find an exact match for the given SMILES string.

        Args:
            smiles (str): The SMILES string to search for.

        Returns:
            dict: A dictionary containing the details of the molecule, or None if no match is found.
        """
        results = await self.molecule_repository.find_similar_molecule(smiles, 1.0, 1)
        if results:
            return results[0]  # Return the first element of the list
        return None  # Return None if no matches were found

    async def findSimilarMolecules(self, smiles: str, similarity: float, limit: int):
        """
        Find molecules similar to the given SMILES string.

        Args:
            smiles (str): The SMILES string to search for.
            similarity (float): The similarity threshold.
            limit (int): The maximum number of results to return.

        Returns:
            list: A list of dictionaries containing the details of the molecules.
        """
        return await self.molecule_repository.find_similar_molecule(
            smiles, similarity, limit
        )

    async def listMolecules(self):
        """
        List all molecules in the database.

        Returns:
            list: A list of dictionaries containing the details of the molecules.
        """
        return await self.molecule_repository.list_molecules()
