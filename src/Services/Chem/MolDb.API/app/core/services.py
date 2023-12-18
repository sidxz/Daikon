from rdkit import Chem
from app.infrastructure.repositories import MoleculeRepository
from rdkit.Chem import rdMolDescriptors, Descriptors
class MoleculeService:
    
    @staticmethod
    def calculate_molecular_properties(smiles_string):
        mol = Chem.MolFromSmiles(smiles_string)
        if mol is not None:
            molecular_weight = Descriptors.MolWt(mol)
            tpsa = rdMolDescriptors.CalcTPSA(mol)
            return molecular_weight, tpsa
        else:
            return None, None
    
    @staticmethod
    async def validate_smiles(smiles: str):
       # Check if smiles is blank or invalid
        if not smiles:
            raise ValueError("SMILES string is required.")
        
        # Check if smiles is valid
        if not Chem.MolFromSmiles(smiles):
            raise ValueError("Invalid SMILES string.")
    

    @staticmethod
    async def create_molecule(name: str, smiles: str):
        # Validate molecule data
        await MoleculeService.validate_smiles(smiles)
        smiles = smiles.upper()
        
        # Calculate Canonical SMILES
        smilesCanonical = Chem.MolToSmiles(Chem.MolFromSmiles(smiles), True)
        
        # Calculate molecular weight and Topological polar surface area (TPSA)
        molecular_weight, tpsa = MoleculeService.calculate_molecular_properties(smiles)
        
        # Add molecule to database
        return await MoleculeRepository.add_molecule(name, smiles, smilesCanonical, molecular_weight, tpsa)
    