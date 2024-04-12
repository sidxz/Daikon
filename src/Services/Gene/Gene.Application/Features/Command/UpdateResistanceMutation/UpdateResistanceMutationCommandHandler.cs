
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateResistanceMutation
{
    public class UpdateResistanceMutationCommandHandler : IRequestHandler<UpdateResistanceMutationCommand, Unit>
    {

        private readonly ILogger<UpdateResistanceMutationCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateResistanceMutationCommandHandler(ILogger<UpdateResistanceMutationCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateResistanceMutationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateResistanceMutationCommandHandler {request}", request);

            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;

            var geneResistanceMutationUpdatedEvent = _mapper.Map<GeneResistanceMutationUpdatedEvent>(request);


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.UpdateResistanceMutation(geneResistanceMutationUpdatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
            }
            
        }
    }
}