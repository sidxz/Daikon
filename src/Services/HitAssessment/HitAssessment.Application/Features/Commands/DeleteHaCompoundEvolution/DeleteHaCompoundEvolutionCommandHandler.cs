using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.DeleteHaCompoundEvolution
{
    public class DeleteHaCompoundEvolutionCommandHandler : IRequestHandler<DeleteHaCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteHaCompoundEvolutionCommandHandler> _logger;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;
        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;

        public DeleteHaCompoundEvolutionCommandHandler(ILogger<DeleteHaCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHaCompoundEvolutionRepository haCompoundEvoRepository,
            IMapper mapper )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));

        }

        public async Task<Unit> Handle(DeleteHaCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.SetUpdateProperties(request.RequestorUserId);
                   
                var haCEDeletedEvent = _mapper.Map<HaCompoundEvolutionDeletedEvent>(request);

                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.DeleteCompoundEvolution(haCEDeletedEvent);

                await _haEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HaAggregate), request.Id);
            }
            return Unit.Value;
        }
    }

}