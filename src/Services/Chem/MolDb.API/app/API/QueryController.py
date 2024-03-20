from ast import List
from uuid import UUID
from fastapi import APIRouter, HTTPException, Depends
from app.Core.MoleculeService import MoleculeService
from app.Infrastructure.MoleculeRepository import MoleculeRepository
import logging
from typing import List


router = APIRouter(prefix="/api/v2/mol-db")
logger = logging.getLogger(__name__)


# Dependency
async def get_molecule_service() -> MoleculeService:
    """
    Dependency provider for MoleculeService.
    """
    
    repository = MoleculeRepository()
    await repository.initialize()  # Initialize if needed
    return MoleculeService(repository)


@router.get("/molecule/", tags=["Queries"])
async def list_molecules(
  molecule_service: MoleculeService = Depends(get_molecule_service),
) -> List[dict]:
  """
  List all molecules.

  Returns:
    List[dict]: A list of all molecules.
  """
  try:
    molecules = await molecule_service.listMolecules()
    return molecules
  except Exception as e:
    logger.error(f"Failed to list molecules: {e}", exc_info=True)
    raise HTTPException(status_code=400, detail=str(e))


@router.get("/molecule/by-id/{id}", tags=["Queries"])
async def get_molecule(
    id: UUID,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Get a molecule by ID.

    Args:
        id (UUID): The ID of the molecule.

    Returns:
        dict: The requested molecule.

    Raises:
        HTTPException: If the molecule is not found.
    """
    try:
        molecule = await molecule_service.readMolecule(id)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))


@router.get("/molecule/by-smiles/{smiles}", tags=["Queries"])
async def get_molecule_by_smiles(
    smiles: str,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Get a molecule by SMILES.

    Args:
        smiles (str): The SMILES representation of the molecule.

    Returns:
        dict: The requested molecule.

    Raises:
        HTTPException: If the molecule is not found.
    """
    try:
        molecule = await molecule_service.readMoleculeBySmile(smiles)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))


@router.get("/molecule/find-exact/{smiles}", tags=["Queries"])
async def find_exact_molecule(
    smiles: str,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Find a exact match by SMILES.

    Args:
        smiles (str): The SMILES representation of the molecule.

    Returns:
        dict: The requested molecule.

    Raises:
        HTTPException: If the molecule is not found.
    """
    try:
        molecule = await molecule_service.findExactMatch(smiles)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))


@router.get("/molecule/find-similar/{smiles}", tags=["Queries"])
async def find_similar_molecule(
    smiles: str,
    threshold: float = 0.8,  # Default value set to 0.8
    limit: int = 10,  # Default value set to return 10 results
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> list:
    """
    Find a similar match by SMILES.

    Args:
        smiles (str): The SMILES representation of the molecule.

    Returns:
        dict: The requested molecule.

    Raises:
        HTTPException: If the molecule is not found.
    """
    try:
        molecule = await molecule_service.findSimilarMolecules(smiles, threshold, limit)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))
