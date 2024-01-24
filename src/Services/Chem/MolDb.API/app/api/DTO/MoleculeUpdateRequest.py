from pydantic import BaseModel
from typing import Optional

class MoleculeUpdateRequest(BaseModel):
    name: Optional[str] = None
    smiles: str