
using CQRS.Core.Exceptions;
using Daikon.Events.HitAssessment;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.HitAssessment;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Handlers
{
    public class HitAssessmentEventHandler : IHitAssessmentEventHandler
    {
        private readonly ILogger<HitAssessmentEventHandler> _logger;
        private readonly IHitAssessmentRepo _graphRepository;

        public HitAssessmentEventHandler(ILogger<HitAssessmentEventHandler> logger, IHitAssessmentRepo graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }
        public Task OnEvent(HaCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: HaCreatedEvent: {@event.Id} {@event.Name}");

            var hitAssessment = new HitAssessment
            {
                UniId = @event.Id.ToString(),
                HitAssessmentId = @event.Id.ToString(),
                Name = @event.Name,
                Status = @event.Status,
                IsHAComplete = @event.IsHAComplete,
                IsHASuccess = @event.IsHASuccess,
                HitCollectionId = @event.HitCollectionId.ToString(),
                PrimaryMoleculeId = @event.CompoundId.ToString(),
                AssociatedMoleculeIds = @event.AssociatedHitIds.Keys.ToList(),
                OrgId = @event.PrimaryOrgId.ToString(),
                DateCreated = @event.DateCreated,
                DateModified = @event.DateModified,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft
            };

            try
            {
                return _graphRepository.Create(hitAssessment);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "HaCreatedEvent Error creating hit assessment", ex);
            }
        }

        public Task OnEvent(HaUpdatedEvent @event)
        {
            var hitAssessment = new HitAssessment
            {
                UniId = @event.Id.ToString(),
                HitAssessmentId = @event.Id.ToString(),
                Name = @event.Name,
                Status = @event.Status,
                IsHAComplete = @event.IsHAComplete,
                IsHASuccess = @event.IsHASuccess,
                OrgId = @event.PrimaryOrgId.ToString(),
                DateCreated = @event.DateCreated,
                DateModified = @event.DateModified,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft
            };

            try
            {
                return _graphRepository.Update(hitAssessment);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "HaUpdatedEvent Error updating hit assessment", ex);
            }
        }

        public Task OnEvent(HaDeletedEvent @event)
        {
            try
            {
                return _graphRepository.Delete(@event.Id.ToString());
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "HaDeletedEvent Error deleting hit assessment", ex);
            }
        }

        public Task OnEvent(HaCompoundEvolutionAddedEvent @event)
        {
            // TODO Implement
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public Task OnEvent(HaCompoundEvolutionUpdatedEvent @event)
        {
            // TODO Implement
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task OnEvent(HaCompoundEvolutionDeletedEvent @event)
        {
            // TODO Implement
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}