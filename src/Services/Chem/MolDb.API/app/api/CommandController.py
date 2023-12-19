from fastapi import APIRouter, HTTPException
from app.Core.MoleculeService import MoleculeService
from app.Infrastructure.MoleculeRepository import MoleculeRepository
router = APIRouter()


@router.post("/molecule/", tags=["Commands"])
async def create_molecule(name: str, smiles: str):
    molecule_repository = MoleculeRepository()
    molecule_service = MoleculeService(molecule_repository)
    res = await molecule_service.createMolecule(name, smiles)
    
    if not res:
        raise HTTPException(status_code=400, detail="Error creating molecule")
    return res
