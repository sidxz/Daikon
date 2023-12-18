from fastapi import APIRouter, HTTPException
from app.infrastructure.repositories import MoleculeRepository

router = APIRouter()

# Example of a query endpoint
# @router.get("/molecule/{name}", tags=["Queries"])
# async def read_molecule(name: str):
#     molecule = await MoleculeRepository.get_molecule_by_name(name)
#     if molecule is None:
#         raise HTTPException(status_code=404, detail="Molecule not found")
#     return molecule
