
using Daikon.Events.MLogix;
using Horizon.Application.Contracts.Persistence;
using Horizon.Domain.MLogix;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Handlers
{
    public class MLogixEventHandler : IMLogixEventHandler
    {
        private readonly ILogger<MLogixEventHandler> _logger;
        private readonly IGraphRepositoryForMLogix _graphRepository;

        public MLogixEventHandler(ILogger<MLogixEventHandler> logger, IGraphRepositoryForMLogix graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public Task OnEvent(MoleculeCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: MoleculeCreatedEvent: {@event.Id} {@event.Name}");
            var molecule = new Molecule
            {
                UniId = @event.Id.ToString(),
                RegistrationId = @event.RegistrationId.ToString(),
                MLogixId = @event.Id.ToString(),
                Name = @event.Name,
                RequestedSMILES = @event.RequestedSMILES,
                SmilesCanonical = @event.SmilesCanonical,

                DateCreated = @event?.DateCreated ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            return _graphRepository.AddMolecule(molecule);
        }

        public Task OnEvent(MoleculeUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: MoleculeUpdatedEvent: {@event.Id} {@event.Name}");
            var molecule = new Molecule
            {
                UniId = @event.Id.ToString(),
                RegistrationId = @event.RegistrationId.ToString(),
                MLogixId = @event.Id.ToString(),
                Name = @event.Name,
                RequestedSMILES = @event.RequestedSMILES,
                SmilesCanonical = @event.SmilesCanonical,

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            return _graphRepository.UpdateMolecule(molecule);
        }
    }
}