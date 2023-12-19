from rdkit import Chem
from rdkit.Chem import rdMolDescriptors, Descriptors


def CalculateMolecularProperties(smiles_string):
    """
    Calculate the molecular weight and TPSA (Topological Polar Surface Area) of a molecule.

    Parameters:
    smiles_string (str): The SMILES string representation of the molecule.

    Returns:
    dict: A dictionary containing the molecular weight and TPSA of the molecule.
          If the SMILES string is blank or invalid, returns {'molecular_weight': None, 'tpsa': None}.
    """
    # check if smiles is blank or invalid
    
    if not smiles_string:
        raise ValueError("calculate_molecular_properties: SMILES string is required.")
    
    mol = Chem.MolFromSmiles(smiles_string)
    
    if mol is not None:
        molecular_weight = Descriptors.MolWt(mol)
        tpsa = rdMolDescriptors.CalcTPSA(mol)
        
        return {'molecular_weight': molecular_weight, 'tpsa': tpsa}
    else:
        return {'molecular_weight': None, 'tpsa': None}
