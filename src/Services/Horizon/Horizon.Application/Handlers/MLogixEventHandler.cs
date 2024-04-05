
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
                UniId =  @event.Id.ToString(),
                RegistrationId = @event.RegistrationId.ToString(),
                MLogixId = @event.Id.ToString(),
                Name = @event.Name,
                RequestedSMILES = @event.RequestedSMILES,
                SmilesCanonical = @event.SmilesCanonical,
                DateCreated = @event.DateCreated,
                DateModified = @event.DateModified,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft
            };

            return _graphRepository.AddMolecule(molecule);
        }
    }
}