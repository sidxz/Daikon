
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

        public HitAssessmentEventHandler(IHitAssessmentRepository haRepository,
                                    IMapper mapper, ILogger<HitAssessmentEventHandler> logger)
        {
            _haRepository = haRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task OnEvent(HaCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HitAssessmentCreatedEvent: {Id}", @event.Id);
            var ha = _mapper.Map<Domain.Entities.HitAssessment>(@event);
            ha.Id = @event.Id;
            ha.DateCreated = DateTime.UtcNow;
            ha.IsModified = false;

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
            var existingHitAssessment = await _haRepository.ReadHaById(@event.Id);

            if (existingHitAssessment == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentUpdatedEvent Error updating ha {@event.Id}", new Exception("HitAssessment not found"));
            }

            var ha = _mapper.Map<Domain.Entities.HitAssessment>(@event);
            ha.DateCreated = existingHitAssessment.DateCreated;
            ha.DateModified = DateTime.UtcNow;
            ha.IsModified = true;

            try
            {
                await _haRepository.UpdateHa(ha);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"HitAssessmentUpdatedEvent Error updating ha {@event.Id}", ex);
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