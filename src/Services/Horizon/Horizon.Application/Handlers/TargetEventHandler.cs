

using CQRS.Core.Exceptions;
using Daikon.Events.Targets;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Targets;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Query.Handlers
{
    public class TargetEventHandler : ITargetEventHandler
    {
        private readonly ILogger<TargetEventHandler> _logger;
        private readonly IGraphRepositoryForTarget _graphRepository;

        public TargetEventHandler(ILogger<TargetEventHandler> logger, IGraphRepositoryForTarget graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public async Task OnEvent(TargetCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: TargetCreatedEvent: {@event.Id} {@event.Name}");
            var target = new Target
            {
                TargetId = @event.Id.ToString(),
                StrainId = @event.StrainId.ToString(),

                Name = @event.Name,
                GeneAccessionNumbers = @event.AssociatedGenes.Values.ToList(),
                TargetType = @event.TargetType,
                Bucket = @event.Bucket,
                
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try {
                 await _graphRepository.AddTargetToGraph(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetCreatedEvent Error creating target", ex);
            }
        }

        public Task OnEvent(TargetUpdatedEvent @event)
        {
            throw new NotImplementedException();
        }

        public Task OnEvent(TargetDeletedEvent @event)
        {
            throw new NotImplementedException();
        }


        // public async Task OnEvent(GeneUpdatedEvent @event)
        // {
        //     _logger.LogInformation($"Horizon: GeneUpdatedEvent: {@event.Id} {@event.AccessionNumber}");
        //      var gene = new Gene
        //     {
        //         GeneId = @event.Id.ToString(),
        //         StrainId = @event.StrainId.ToString(),

        //         Name = @event.Name,
        //         AccessionNumber = @event.AccessionNumber,
        //         Function = @event.Function,
        //         Product = @event.Product,
        //         FunctionalCategory = @event.FunctionalCategory,

        //         DateCreated = DateTime.UtcNow,
        //         IsModified = true,
        //         IsDraft = false
        //     };

        //     try {
        //          await _graphRepository.UpdateGeneOfGraph(gene);
        //     }
        //     catch (RepositoryException ex)
        //     {
        //         throw new EventHandlerException(nameof(EventHandler), "GeneCreatedEvent Error creating gene", ex);
        //     }

        // }




        // public async Task OnEvent(GeneDeletedEvent @event)
        // {
        //     _logger.LogInformation($"Horizon: GeneDeletedEvent: {@event.Id} {@event.AccessionNumber}");
        //     try {
        //          await _graphRepository.DeleteGeneFromGraph(@event.Id.ToString());
        //     }
        //     catch (RepositoryException ex)
        //     {
        //         throw new EventHandlerException(nameof(EventHandler), "GeneDeletedEvent Error deleting gene", ex);
        //     }
        // }
    }
}