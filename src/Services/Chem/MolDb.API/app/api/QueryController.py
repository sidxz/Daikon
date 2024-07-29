from fastapi import APIRouter, HTTPException, Depends, Request
from app.core.MoleculeService import MoleculeService
from app.infrastructure.MoleculeRepository import MoleculeRepository
import logging
from typing import List
from uuid import UUID

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

def log_request_headers(request: Request):
    """
    Logs the request headers.
    """
    logger.info("Request Headers:")
    print("Request Headers:")
    for key, value in request.headers.items():
        logger.info(f"{key}: {value}")
        print(f"{key}: {value}")

@router.get("/molecule/", tags=["Queries"])
async def list_molecules(
    request: Request,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> List[dict]:
    """
    List all molecules.

    Returns:
        List[dict]: A list of all molecules.
    """
    log_request_headers(request)
    try:
        molecules = await molecule_service.listMolecules()
        return molecules
    except Exception as e:
        logger.error(f"Failed to list molecules: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))

@router.get("/molecule/by-id/{id}", tags=["Queries"])
async def get_molecule(
    id: UUID,
    request: Request,
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
    log_request_headers(request)
    try:
        molecule = await molecule_service.readMolecule(id)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))

@router.get("/molecule/by-smiles", tags=["Queries"])
async def get_molecule_by_smiles(
    smiles: str,
    request: Request,
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
    log_request_headers(request)
    try:
        molecule = await molecule_service.readMoleculeBySmile(smiles)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))

@router.get("/molecule/find-exact", tags=["Queries"])
async def find_exact_molecule(
    smiles: str,
    request: Request,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Find an exact match by SMILES.

    Args:
        smiles (str): The SMILES representation of the molecule.

    Returns:
        dict: The requested molecule.

    Raises:
        HTTPException: If the molecule is not found.
    """
    log_request_headers(request)
    try:
        molecule = await molecule_service.findExactMatch(smiles)
        if molecule is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return molecule
    except Exception as e:
        logger.error(f"Failed to retrieve molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))

@router.get("/molecule/find-similar", tags=["Queries"])
async def find_similar_molecule(
    smiles: str,
    request: Request,
    threshold: float = 0.8,  # Default value set to 0.8
    limit: int = 30,  # Default value set to return 10 results
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> List[dict]:
    """
    Find a similar match by SMILES.

    Args:
        smiles (str): The SMILES representation of the molecule.
        threshold (float): Similarity threshold.
        limit (int): Number of results to return.

    Returns:
        list: List of similar molecules.

    Raises:
        HTTPException: If no similar molecules are found or on error.
    """
    log_request_headers(request)
    try:
        molecules = await molecule_service.findSimilarMolecules(smiles, threshold, limit)
        if not molecules:
            raise HTTPException(status_code=404, detail="No similar molecules found")
        return molecules
    except Exception as e:
        logger.error(f"Failed to retrieve molecules: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))
