from rdkit import Chem

async def ValidateSmiles(smiles: str):
    """
    Validates a SMILES string.

    Args:
        smiles (str): The SMILES string to validate.

    Raises:
        ValueError: If the SMILES string is empty or invalid.
    """
    if not smiles:
        raise ValueError("SMILES string is required.")
    
    if not Chem.MolFromSmiles(smiles):
        raise ValueError("Invalid SMILES string.")
