using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using System;

namespace Screen.Application.Features.Commands.UpdateHitCollectionAssociatedScreen
{
    public class UpdateHitCollectionAssociatedScreenCommandHandler : IRequestHandler<UpdateHitCollectionAssociatedScreenCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHitCollectionAssociatedScreenCommandHandler> _logger;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;

        public UpdateHitCollectionAssociatedScreenCommandHandler(ILogger<UpdateHitCollectionAssociatedScreenCommandHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitCollectionRepository hitCollectionRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionRepository = hitCollectionRepository ?? throw new ArgumentNullException(nameof(hitCollectionRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateHitCollectionAssociatedScreenCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            _logger.LogInformation("Updating hit collection associated screen. Id: {Id}", request.Id);
            try
            {
                var hitCollectionAssociatedScreenUpdatedEvent = _mapper.Map<HitCollectionAssociatedScreenUpdatedEvent>(request);

                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateHitCollectionAssociatedScreen(hitCollectionAssociatedScreenUpdatedEvent);

                await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);

            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hit collection associated screen. Id: {Id}", request.Id);
                throw;
            }

            return Unit.Value;
        }
    }
}