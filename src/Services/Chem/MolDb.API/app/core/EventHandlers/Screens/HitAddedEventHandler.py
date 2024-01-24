import logging
from app.Core.MoleculeService import MoleculeService
from app.Infrastructure.MoleculeRepository import MoleculeRepository

logger = logging.getLogger(__name__)

async def HandleHitAddedEvent(event_data):
    """
    Process the HitAddedEvent using MoleculeService.

    Args:
        event_data (dict): Data containing the SMILES of the molecule.
    """
    logger.info(f"Handling HitAddedEvent: {event_data}")
    
    # Extract SMILES from event_data
    smiles = event_data.get('InitialCompoundStructure')['Value']
    if not smiles:
        logger.warning("No SMILES data found in the event.")
        return

    # Instantiate the repository and molecule service
    repository = MoleculeRepository()
    await repository.initialize()  # Initialize if needed
    molecule_service = MoleculeService(repository)

    # Check if SMILES matches any existing molecule using MoleculeService
    existing_molecule = await molecule_service.findExactMatch(smiles)

    if existing_molecule:
        # If match found, handle MoleculeMatchedEvent (implement your logic)
        logger.info(f"Molecule matched: {existing_molecule['id']}")
        # Handle MoleculeMatchedEvent...
    else:
        # If no match, add the molecule (additional details like name should be handled)
        new_molecule = await molecule_service.createMolecule("New Molecule", smiles)
        logger.info(f"New molecule added: {new_molecule['id']}")
        # Handle MoleculeAddedEvent...
