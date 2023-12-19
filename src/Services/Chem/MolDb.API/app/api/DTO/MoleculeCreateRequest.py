from pydantic import BaseModel
from typing import Optional

class MoleculeCreateRequest(BaseModel):
    name: Optional[str] = None
    smiles: str
