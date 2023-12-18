from marshmallow import Schema, fields, validate

class Molecule(Schema):
    id = fields.UUID()  # Using UUID for GUID
    name = fields.Str()
    smiles = fields.Str()
    smiles_canonical = fields.Str()
    molecular_weight = fields.Float(as_string=True)  # Using Float for double, as_string=True to serialize as string if needed
    tpsa = fields.Float(as_string=True)  # Same as above