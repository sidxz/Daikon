
using AutoMapper;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using HitAssessment.Application.Contracts.Persistence;
using Daikon.Events.HitAssessment;

namespace HitAssessment.Application.EventHandlers
{
    public partial class HitAssessmentEventHandler : IHitAssessmentEventHandler
    {

        private readonly IHitAssessmentRepository _haRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HitAssessmentEventHandler> _logger;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvolutionRepository;

        public HitAssessmentEventHandler(IHitAssessmentRepository haRepository, IHaCompoundEvolutionRepository haCompoundEvolutionRepository,
                                    IMapper mapper, ILogger<HitAssessmentEventHandler> logger)
        {
            _haRepository = haRepository;
            _haCompoundEvolutionRepository = haCompoundEvolutionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task OnEvent(HaCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitAssessmentCreatedEvent: {Id}", @event.Id);
            var ha = _mapper.Map<Domain.Entities.HitAssessment>(@event);
            ha.Id = @event.Id;
            try
            {
                await _haRepository.CreateHa(ha);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "HitAssessmentCreatedEvent Error creating ha", ex);
            }
        }

        public async Task OnEvent(HaUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitAssessmentUpdatedEvent: {Id}", @event.Id);
            var existingHa = await _haRepository.ReadHaById(@event.Id);

            if (existingHa == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentUpdatedEvent Error updating ha {@event.Id}", new Exception("HitAssessment not found"));
            }

            var ha = _mapper.Map<Domain.Entities.HitAssessment>(existingHa);
            _mapper.Map(@event, ha);

            // Preserve the original creation date and creator
            ha.DateCreated = existingHa.DateCreated;
            ha.CreatedById = existingHa.CreatedById;

            try
            {
                await _haRepository.UpdateHa(ha);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentUpdatedEvent Error updating ha {@event.Id}", ex);
            }
        }

        public async Task OnEvent(HaRenamedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitAssessmentRenamedEvent: {Id}", @event.Id);
            var existingHa = await _haRepository.ReadHaById(@event.Id);

            if (existingHa == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentRenamedEvent Error renaming ha {@event.Id}", new Exception("HitAssessment not found"));
            }

            var ha = _mapper.Map<Domain.Entities.HitAssessment>(existingHa);
            _mapper.Map(@event, ha);
            
            // Preserve the original creation date and creator
            ha.DateCreated = existingHa.DateCreated;
            ha.CreatedById = existingHa.CreatedById;

            try
            {
                await _haRepository.UpdateHa(ha);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentRenamedEvent Error renaming ha {@event.Id}", ex);
            }
        }

        public async Task OnEvent(HaDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitAssessmentDeletedEvent: {Id}", @event.Id);
            try
            {
                await _haRepository.DeleteHa(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentDeletedEvent Error deleting ha {@event.Id}", ex);
            }

        }

    }
}