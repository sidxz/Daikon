
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
                UniId = @event.Id.ToString(),
                TargetId = @event.Id.ToString(),
                StrainId = @event.StrainId.ToString(),

                Name = @event.Name,
                GeneAccessionNumbers = @event.AssociatedGenes.Values.ToList(),
                TargetType = @event.TargetType,
                Bucket = @event.Bucket,

                DateCreated = @event?.DateCreated ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            try
            {
                await _graphRepository.AddTarget(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetCreatedEvent Error creating target", ex);
            }
        }

        public async Task OnEvent(TargetUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: TargetUpdatedEvent: {@event.Id} {@event.Name}");
            var target = new Target
            {
                UniId = @event.Id.ToString(),
                TargetId = @event.Id.ToString(),
                StrainId = @event.StrainId.ToString(),

                Name = @event.Name,
                GeneAccessionNumbers = @event.AssociatedGenes.Values.ToList(),
                TargetType = @event.TargetType,
                Bucket = @event.Bucket,

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            try
            {
                await _graphRepository.UpdateTarget(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetUpdatedEvent Error updating target", ex);
            }
        }

        public async Task OnEvent(TargetAssociatedGenesUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: TargetAssociatedGenesUpdatedEvent: {@event.Id} {@event.Name}");
            var target = new Target
            {
                UniId = @event.Id.ToString(),
                TargetId = @event.Id.ToString(),
                Name = @event.Name,
                GeneAccessionNumbers = @event.AssociatedGenes.Values.ToList(),
                TargetType = @event.TargetType,

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };
            try
            {
                await _graphRepository.UpdateAssociatedGenesOfTarget(target);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetAssociatedGenesUpdatedEvent Error updating target", ex);
            }
        }

        public Task OnEvent(TargetDeletedEvent @event)
        {
            _logger.LogInformation($"Horizon: TargetDeletedEvent: {@event.Id} {@event.Name}");
            try
            {
                return _graphRepository.DeleteTarget(@event.Id.ToString());
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetDeletedEvent Error deleting target", ex);
            }
        }

        public async Task OnEvent(TargetRenamedEvent @event)
        {
            _logger.LogInformation($"Horizon: TargetRenamedEvent: {@event.Id} {@event.Name}");


            try
            {
                await _graphRepository.RenameTarget(@event.Id.ToString(), @event.Name);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetRenamedEvent Error renaming target", ex);
            }
        }



        // public async Task OnEvent(GeneDeletedEvent @event)
        // {
        //     _logger.LogInformation($"Horizon: GeneDeletedEvent: {@event.Id} {@event.AccessionNumber}");
        //     try {
        //          await _graphRepository.DeleteGene(@event.Id.ToString());
        //     }
        //     catch (RepositoryException ex)
        //     {
        //         throw new EventHandlerException(nameof(EventHandler), "GeneDeletedEvent Error deleting gene", ex);
        //     }
        // }
    }
}