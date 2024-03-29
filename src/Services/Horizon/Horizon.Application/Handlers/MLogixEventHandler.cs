
using Daikon.Events.MLogix;
using Horizon.Application.Contracts.Persistance;
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
            _logger.LogInformation($"Horizon: MoleculeCreatedEvent: {@event.RegistrationId} {@event.Name}");
            var molecule = new Molecule
            {
                RegistrationId = @event.RegistrationId.ToString(),
                MLogixId = @event.Id.ToString(),
                Name = @event.Name,
                RequestedSMILES = @event.RequestedSMILES,
                SmilesCanonical = @event.SmilesCanonical,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
            };

            return _graphRepository.AddMolecule(molecule);
        }
    }
}