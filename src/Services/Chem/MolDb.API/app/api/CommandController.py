from fastapi import APIRouter, HTTPException, Depends
from app.Core.MoleculeService import MoleculeService
from app.Infrastructure.MoleculeRepository import MoleculeRepository
from app.API.DTO.MoleculeCreateRequest import MoleculeCreateRequest
import logging


router = APIRouter(prefix="/api/v2")
logger = logging.getLogger(__name__)


# Dependency
async def get_molecule_service() -> MoleculeService:
    """
    Dependency provider for MoleculeService.
    """
    repository = MoleculeRepository()
    await repository.initialize()  # Initialize if needed
    return MoleculeService(repository)


@router.post("/molecule/", tags=["Commands"])
async def create_molecule(
    request: MoleculeCreateRequest,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Create a new molecule.

    Args:
        request (MoleculeCreateRequest): The request model containing the name and SMILES of the molecule.

    Returns:
        dict: The created molecule.

    Raises:
        HTTPException: If there is an error creating the molecule.
    """
    logger.info(f"Creating molecule: {request}")
    try:
        res = await molecule_service.createMolecule(request.name, request.smiles)
        return res
    except Exception as e:
        logger.error(f"Failed to create molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))
