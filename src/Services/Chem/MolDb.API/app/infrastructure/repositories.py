from app.infrastructure.database import get_db
from app.dto.Molecule import Molecule


class MoleculeRepository:
    async def add_molecule(
        self,
        name: str,
        smiles: str,
        smiles_canonical: str,
        molecular_weight: float,
        tpsa: float,
    ) -> dict:
        # Check if smiles is blank or invalid
        if not smiles:
            raise ValueError("SMILES string is required.")

        # Make smiles string uppercase
        smiles = smiles.upper()

        async with get_db() as conn:
            # Execute the insert query
            result = await conn.fetchrow(
                """
                INSERT INTO molecules (name, smiles, smiles_canonical, mol, mfp2, molecular_weight, tpsa)
                VALUES ($1, $2, $3, mol_from_smiles($4::cstring), morganbv_fp(mol_from_smiles($5::cstring)), $6, $7)
                RETURNING id;
                """,
                name,
                smiles,
                smiles_canonical,
                smiles,
                smiles,
                molecular_weight,
                tpsa,
            )

            # Extract the molecule ID from the result
            mol_id = result["id"]

            molecule = Molecule()

            return molecule.dump(
                {
                    "id": mol_id,
                    "name": name,
                    "smiles": smiles,
                    "smiles_canonical": smiles_canonical,
                    "molecular_weight": molecular_weight,
                    "tpsa": tpsa,
                }
            )
