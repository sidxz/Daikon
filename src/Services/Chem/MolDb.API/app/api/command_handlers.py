from fastapi import APIRouter, HTTPException
from app.core.services import MoleculeService

router = APIRouter()

# Example of a command endpoint
@router.post("/molecule/", tags=["Commands"])
async def create_molecule(name: str, smiles: str):
    res = await MoleculeService.create_molecule(name, smiles)
    if not res:
        raise HTTPException(status_code=400, detail="Error creating molecule")
    return res
