from fastapi import FastAPI, HTTPException
from rdkit import Chem
from rdkit.Chem import rdMolDescriptors, Descriptors
import psycopg2
import psycopg2.extras

app = FastAPI()

DATABASE_URL = "postgresql://user:password@MolDb_Postgres:5432/chemdb"


def get_db():
    conn = psycopg2.connect(DATABASE_URL)
    return conn


def setup_database():
    conn = get_db()
    cursor = conn.cursor()

    # Enable RDKit extension if not already enabled
    cursor.execute("SELECT 1 FROM pg_extension WHERE extname = 'rdkit';")
    if not cursor.fetchone():
        cursor.execute("CREATE EXTENSION rdkit;")

    # Create the molecules table if it doesn't exist
    cursor.execute(
        """
        DO $$ BEGIN
            IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'molecules') THEN
                CREATE TABLE molecules (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100),
                    smiles VARCHAR(500),
                    molecular_weight DOUBLE PRECISION,
                    tpsa DOUBLE PRECISION,
                    mol MOL,
                    mfp2 BFP
                    
                );
                CREATE INDEX mol_idx ON molecules USING gist (mol);
                CREATE INDEX mfp2_idx ON molecules USING gist (mfp2);

            END IF;
        END $$;
        """
    )

    conn.commit()
    cursor.close()
    conn.close()


# Call the setup function when the script starts
setup_database()

# Function to calculate the molecular properties such as molecular weight and Topological polar surface area (TPSA) of a molecule
def calculate_molecular_properties(smiles_string):
    mol = Chem.MolFromSmiles(smiles_string)
    if mol is not None:
        molecular_weight = Descriptors.MolWt(mol)
        tpsa = rdMolDescriptors.CalcTPSA(mol)
        return molecular_weight, tpsa
    else:
        return None, None

@app.post("/molecule/")
def create_molecule(name: str, smiles: str):
    conn = get_db()
    cursor = conn.cursor()

    # Calculate molecular weight and Topological polar surface area (TPSA)
    molecular_weight, tpsa = calculate_molecular_properties(smiles)

    if molecular_weight is not None and tpsa is not None:
        cursor.execute(
            """
            INSERT INTO molecules (name, smiles, mol, mfp2, molecular_weight, tpsa)
            VALUES (%s, %s, mol_from_smiles(%s::cstring), morganbv_fp(mol_from_smiles(%s::cstring)), %s, %s)
            RETURNING id;
            """,
            (name, smiles, smiles, smiles, molecular_weight, tpsa),
        )


        mol_id = cursor.fetchone()[0]
        conn.commit()
        cursor.close()
        conn.close()
        return {"id": mol_id, "name": name, "smiles": smiles, "molecular_weight": molecular_weight, "tpsa": tpsa}
    else:
        raise HTTPException(status_code=400, detail=f"Invalid SMILES string: {smiles}")


@app.get("/molecule/{name}")
def read_molecule(name: str):
    conn = get_db()
    cursor = conn.cursor(cursor_factory=psycopg2.extras.DictCursor)
    cursor.execute("SELECT id, name, smiles, molecular_weight, tpsa FROM molecules WHERE name = %s;", (name,))
    mol = cursor.fetchone()
    cursor.close()
    conn.close()
    if mol is None:
        raise HTTPException(status_code=404, detail="Molecule not found")
    return {"id": mol["id"], "name": mol["name"], "smiles": mol["smiles"],
            "molecular_weight": mol["molecular_weight"], "tpsa": mol["tpsa"]}


@app.get("/molecule/smiles/{smiles}")
def read_molecule_by_smiles(smiles: str):
    conn = get_db()
    cursor = conn.cursor(cursor_factory=psycopg2.extras.DictCursor)
    cursor.execute(
        "SELECT id, name, smiles, molecular_weight, tpsa FROM molecules WHERE smiles = %s;", (smiles,)
    )
    mol = cursor.fetchone()
    cursor.close()
    conn.close()
    if mol is None:
        raise HTTPException(status_code=404, detail="Molecule not found")
    return {"id": mol["id"], "name": mol["name"], "smiles": mol["smiles"],
            "molecular_weight": mol["molecular_weight"], "tpsa": mol["tpsa"]}


@app.put("/molecule/{name}")
def update_molecule(name: str, new_smiles: str):
    conn = get_db()
    cursor = conn.cursor()


    # Calculate molecular weight and TPSA for the new SMILES
    molecular_weight, tpsa = calculate_molecular_properties(new_smiles)

    if molecular_weight is not None and tpsa is not None:
        cursor.execute(
            "UPDATE molecules SET smiles = %s, molecular_weight = %s, tpsa = %s WHERE name = %s RETURNING id;",
            (new_smiles, molecular_weight, tpsa, name),
        )
        mol_id = cursor.fetchone()
        conn.commit()
        cursor.close()
        conn.close()
        if mol_id is None:
            raise HTTPException(status_code=404, detail="Molecule not found")
        return {"id": mol_id[0], "name": name, "smiles": new_smiles, "molecular_weight": molecular_weight, "tpsa": tpsa}
    else:
        raise HTTPException(status_code=400, detail=f"Invalid SMILES string: {new_smiles}")


@app.delete("/molecule/{name}")
def delete_molecule(name: str):
    conn = get_db()
    cursor = conn.cursor()
    cursor.execute("DELETE FROM molecules WHERE name = %s RETURNING id;", (name,))
    mol_id = cursor.fetchone()
    conn.commit()
    cursor.close()
    conn.close()
    if mol_id is None:
        raise HTTPException(status_code=404, detail="Molecule not found")
    return {"message": "Molecule deleted successfully"}


@app.get("/molecule/similar/{smiles}")
def find_similar_molecule(smiles: str, threshold: float = 0.8):
    conn = get_db()
    cursor = conn.cursor(cursor_factory=psycopg2.extras.DictCursor)

    # Using the % operator for similarity search and incorporating the threshold
    cursor.execute(
    """
    SELECT id, name, smiles, molecular_weight, tpsa
    FROM molecules 
    WHERE tanimoto_sml(morganbv_fp(mol_from_smiles(%s::cstring)), mfp2) >= %s
    ORDER BY tanimoto_sml(morganbv_fp(mol_from_smiles(%s::cstring)), mfp2) DESC
    LIMIT 10;
    """, 
    (smiles, threshold, smiles)
)

    similar_mols = cursor.fetchall()
    cursor.close()
    conn.close()

    if not similar_mols:
        raise HTTPException(status_code=404, detail="Similar molecules not found")

    return [
        {"id": mol["id"], "name": mol["name"], "smiles": mol["smiles"], "molecular_weight": mol["molecular_weight"],
            "tpsa": mol["tpsa"],
        }
        for mol in similar_mols
    ]

