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
        pass
