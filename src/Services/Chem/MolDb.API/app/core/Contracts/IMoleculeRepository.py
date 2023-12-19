from abc import ABC, abstractmethod
from typing import Dict


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
