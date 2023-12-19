from marshmallow import Schema, fields, validate


class Molecule(Schema):
    id = fields.UUID(required=True, description="Unique Identifier for the molecule")
    name = fields.Str(
        required=False,
        validate=validate.Length(max=255),
        description="Name of the molecule",
    )
    smiles = fields.Str(
        required=True,
        validate=validate.Length(max=500),
        description="SMILES representation of the molecule",
    )
    smilesCanonical = fields.Str(
        required=False,
        validate=validate.Length(max=500),
        description="Canonical SMILES representation",
    )
    molecularWeight = fields.Float(
        as_string=False,
        validate=validate.Range(min=0),
        description="Molecular weight of the molecule",
    )
    tpsa = fields.Float(
        as_string=False,
        validate=validate.Range(min=0),
        description="Topological Polar Surface Area",
    )

    class Meta:
        ordered = (
            True  # Ensures the fields are serialized in the order they are declared
        )
