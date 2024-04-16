
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateProteinActivityAssay
{
    public class UpdateProteinActivityAssayCommandHandler : IRequestHandler<UpdateProteinActivityAssayCommand, Unit>
    {

        private readonly ILogger<UpdateProteinActivityAssayCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateProteinActivityAssayCommandHandler(ILogger<UpdateProteinActivityAssayCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateProteinActivityAssayCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateProteinActivityAssayCommandHandler {request}", request);

            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;

            var geneProteinActivityAssayUpdatedEvent = _mapper.Map<GeneProteinActivityAssayUpdatedEvent>(request);
            geneProteinActivityAssayUpdatedEvent.LastModifiedById = request.RequestorUserId;


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.UpdateProteinActivityAssay(geneProteinActivityAssayUpdatedEvent);
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