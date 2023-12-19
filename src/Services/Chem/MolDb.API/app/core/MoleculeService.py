from rdkit import Chem

from app.Core.MoleculeServices.MolecularProperties import CalculateMolecularProperties
from app.Core.MoleculeServices.SmilesValidation import ValidateSmiles

class MoleculeService:
    def __init__(self, molecule_repository):
        self.molecule_repository = molecule_repository
        
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
        await ValidateSmiles(smiles)
        smiles = smiles.upper()

        # Calculate Canonical SMILES
        smilesCanonical = Chem.MolToSmiles(Chem.MolFromSmiles(smiles), True)

        # Calculate molecular weight and Topological polar surface area (TPSA)
        molecular_properties = CalculateMolecularProperties(smiles)

        # Add molecule to database
        return await self.molecule_repository.add_molecule(
            name,
            smiles,
            smilesCanonical,
            molecular_properties["molecular_weight"],
            molecular_properties["tpsa"],
        )
