using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.MLogix;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Entities;

namespace MLogix.Application.EventHandlers
{
    public partial class MLogixEventHandler : IMLogixEventHandler
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMoleculePredictionRepository _moleculePredictionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MLogixEventHandler> _logger;

        public MLogixEventHandler(IMoleculeRepository moleculeRepository, IMoleculePredictionRepository moleculePredictionRepository, IMapper mapper, ILogger<MLogixEventHandler> logger)
        {
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculePredictionRepository = moleculePredictionRepository ?? throw new ArgumentNullException(nameof(moleculePredictionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnEvent(MoleculeCreatedEvent @event)
        {
            _logger.LogInformation("MoleculeCreatedEvent received for molecule {moleculeId}", @event.Id);
            var molecule = _mapper.Map<Molecule>(@event);
            molecule.Id = @event.Id;

            try
            {
                await _moleculeRepository.NewMolecule(molecule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the molecule with ID {moleculeId}", @event.Id);
                throw new EventHandlerException(nameof(MLogixEventHandler), "Error creating molecule", ex);
            }

        }

        public async Task OnEvent(MoleculeUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: MoleculeUpdatedEvent: {Id}", @event.Id);
            var existingMolecule = await _moleculeRepository.GetMoleculeById(@event.Id);

            if (existingMolecule == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"MoleculeUpdatedEvent Error updating molecule with id @event.Id", new Exception("Molecule not found"));
            }

            var molecule = _mapper.Map<Molecule>(existingMolecule);
            _mapper.Map(@event, molecule);

            // Preserve the original creation date and creator
            molecule.DateCreated = existingMolecule.DateCreated;
            molecule.CreatedById = existingMolecule.CreatedById;

            try
            {
                await _moleculeRepository.UpdateMolecule(molecule);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "MoleculeUpdatedEvent Error updating molecule with id @event.Id", ex);
            }
        }

        public async Task OnEvent(MoleculeDisclosedEvent @event)
        {
            _logger.LogInformation("OnEvent: MoleculeDisclosedEvent: {Id}", @event.Id);
            var existingMolecule = await _moleculeRepository.GetMoleculeById(@event.Id);

            if (existingMolecule == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"MoleculeDisclosedEvent Error updating molecule with id @event.Id", new Exception("Molecule not found"));
            }

            var molecule = _mapper.Map<Molecule>(existingMolecule);
            _mapper.Map(@event, molecule);

            // Preserve the original creation date and creator
            molecule.DateCreated = existingMolecule.DateCreated;
            molecule.CreatedById = existingMolecule.CreatedById;

            try
            {
                await _moleculeRepository.UpdateMolecule(molecule);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "MoleculeDisclosedEvent Error updating molecule with id @event.Id", ex);
            }
        }

        public Task OnEvent(MoleculeDeletedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}