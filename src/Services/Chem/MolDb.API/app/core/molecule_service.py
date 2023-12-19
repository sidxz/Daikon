from rdkit import Chem
from app.infrastructure.repositories import MoleculeRepository
from app.core.molecule_services.molecular_properties import (
    calculate_molecular_properties,
)
from app.core.molecule_services.smiles_validation import validate_smiles


class MoleculeService:
    @staticmethod
    async def create_molecule(name: str, smiles: str):
        """
        Create a molecule with the given name and SMILES representation.

        Args:
            name (str): The name of the molecule.
            smiles (str): The SMILES representation of the molecule.

        Returns:
            dict: A dictionary containing the details of the created molecule.
        """
        # Validate molecule data
        await validate_smiles(smiles)
        smiles = smiles.upper()

        # Calculate Canonical SMILES
        smilesCanonical = Chem.MolToSmiles(Chem.MolFromSmiles(smiles), True)

        # Calculate molecular weight and Topological polar surface area (TPSA)
        molecular_properties = calculate_molecular_properties(smiles)

        # Add molecule to database
        return await MoleculeRepository.add_molecule(
            name,
            smiles,
            smilesCanonical,
            molecular_properties["molecular_weight"],
            molecular_properties["tpsa"],
        )
