from abc import ABC, abstractmethod
from typing import Dict
from uuid import UUID


class IMoleculeRepository(ABC):
    @abstractmethod
    async def add_molecule(
        self,
        name: str,
        smiles: str,
        smiles_canonical: str,
        molecular_weight: float,
        tpsa: float,
    ) -> Dict:
        """
        Adds a molecule to the repository.

        Args:
            name (str): The name of the molecule.
            smiles (str): The SMILES representation of the molecule.
            smiles_canonical (str): The canonical SMILES representation of the molecule.
            molecular_weight (float): The molecular weight of the molecule.
            tpsa (float): The topological polar surface area of the molecule.

        Returns:
            Dict: A dictionary containing the details of the added molecule.
        """
        pass

    @abstractmethod
    async def read_molecule(self, id: UUID) -> Dict:
        """
        Reads a molecule from the repository.

        Args:
            id (UUID): The ID of the molecule.

        Returns:
            Dict: A dictionary containing the details of the molecule.
        """
        pass

    @abstractmethod
    async def read_molecule_by_smiles(self, smiles: str) -> Dict:
        """
        Reads a molecule from the repository by its SMILES representation.

        Args:
            smiles (str): The SMILES representation of the molecule.

        Returns:
            Dict: A dictionary containing the details of the molecule.
        """
        pass

    @abstractmethod
    async def update_molecule(
        self,
        id: UUID,
        name: str,
        smiles: str,
        smiles_canonical: str,
        molecular_weight: float,
        tpsa: float,
    ) -> Dict:
        """
        Updates a molecule in the repository.

        Args:
            name (str): The name of the molecule.
            new_smiles (str): The new SMILES representation of the molecule.

        Returns:
            Dict: A dictionary containing the details of the updated molecule.
        """
        pass

    @abstractmethod
    async def delete_molecule(self, id: UUID) -> Dict:
        """
        Deletes a molecule from the repository.

        Args:
            id (UUID): The ID of the molecule.

        Returns:
            Dict: A dictionary containing the details of the deleted molecule.
        """
        pass

    @abstractmethod
    async def find_similar_molecule(self, smiles: str, threshold: float, num_results: int) -> Dict:
        """
        Finds a similar molecule in the repository.

        Args:
            smiles (str): The SMILES representation of the molecule.
            threshold (float): The similarity threshold.

        Returns:
            Dict: A dictionary containing the details of the similar molecule.
        """
        pass

    @abstractmethod
    async def list_molecules(self) -> Dict:
        """
        Lists all molecules in the repository.

        Returns:
            Dict: A dictionary containing the details of the molecules.
        """
        pass
