from uuid import UUID
from fastapi import APIRouter, HTTPException, Depends
from app.core.MoleculeService import MoleculeService
from app.infrastructure.MoleculeRepository import MoleculeRepository
from app.api.DTO.MoleculeUpdateRequest import MoleculeUpdateRequest
from app.api.DTO.MoleculeCreateRequest import MoleculeCreateRequest
import logging


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


@router.put("/molecule/{molecule_id}", tags=["Commands"])
async def update_molecule(
    molecule_id: UUID,
    request: MoleculeUpdateRequest,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Update an existing molecule.

    Args:
        molecule_id (UUID): The ID of the molecule to update.
        request (MoleculeUpdateRequest): The request model containing updated molecule data.

    Returns:
        dict: The updated molecule.

    Raises:
        HTTPException: If there is an error updating the molecule.
    """
    logger.info(f"Updating molecule {molecule_id} with data: {request}")
    try:
        res = await molecule_service.updateMolecule(
            molecule_id, request.name, request.smiles
        )
        return res
    except Exception as e:
        logger.error(f"Failed to update molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))

@router.delete("/molecule/{molecule_id}", tags=["Commands"])
async def delete_molecule(
    molecule_id: UUID,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Delete an existing molecule.

    Args:
        molecule_id (UUID): The ID of the molecule to delete.

    Returns:
        dict: The deleted molecule.

    Raises:
        HTTPException: If there is an error deleting the molecule.
    """
    logger.info(f"Deleting molecule {molecule_id}")
    try:
        res = await molecule_service.deleteMolecule(molecule_id)
        return res
    except Exception as e:
        logger.error(f"Failed to delete molecule: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))
    
    
@router.post("/molecule/register", tags=["Commands"])
async def register_molecule(
    request: MoleculeCreateRequest,
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Create a new molecule based on smile or return an existing.

    Args:
        request (MoleculeCreateRequest): The request model containing the name and SMILES of the molecule.

    Returns:
        dict: The created molecule.

    Raises:
        HTTPException: If there is an error creating the molecule.
    """
    logger.info(f"Registering molecule: {request}")
    # Extract SMILES
    if not request.smiles:
        logger.warning("No SMILES data found in the request.")
        raise HTTPException(status_code=400, detail="No SMILES data found in the request.")
    # Check if SMILES matches any existing molecule using MoleculeService
    existing_molecule = await molecule_service.findExactMatch(request.smiles)
    
    if existing_molecule:
        logger.info(f"Molecule matched to existing: {existing_molecule['id']}")
        return existing_molecule
    # Handle MoleculeMatchedEvent...
    else:
    # If no match, add the molecule (additional details like name should be handled)
        try:
            new_molecule = await molecule_service.createMolecule(request.name, request.smiles)
            logger.info(f"New molecule added: {new_molecule['id']}")
            return new_molecule
        except Exception as e:
            logger.error(f"Failed to create molecule: {e}", exc_info=True)
            raise HTTPException(status_code=400, detail=str(e))

# Recalculate fingerprints for all molecules in the database
@router.post("/molecule/recalculate-fingerprints", tags=["Commands"])
async def recalculate_fingerprints(
    molecule_service: MoleculeService = Depends(get_molecule_service),
) -> dict:
    """
    Recalculate fingerprints for all molecules in the database.

    Returns:
        dict: The result of the operation.

    Raises:
        HTTPException: If there is an error recalculating fingerprints.
    """
    logger.info("Recalculating fingerprints for all molecules")
    try:
        await molecule_service.recalculateFingerprints()
        return {"message": "Fingerprints recalculated successfully."}
    except Exception as e:
        logger.error(f"Failed to recalculate fingerprints: {e}", exc_info=True)
        raise HTTPException(status_code=400, detail=str(e))