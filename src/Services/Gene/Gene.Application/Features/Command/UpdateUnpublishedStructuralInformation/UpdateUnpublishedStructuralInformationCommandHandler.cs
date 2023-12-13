
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateUnpublishedStructuralInformation
{
    public class UpdateUnpublishedStructuralInformationCommandHandler : IRequestHandler<UpdateUnpublishedStructuralInformationCommand, Unit>
    {

        private readonly ILogger<UpdateUnpublishedStructuralInformationCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;

        public UpdateUnpublishedStructuralInformationCommandHandler(ILogger<UpdateUnpublishedStructuralInformationCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateUnpublishedStructuralInformationCommand request, CancellationToken cancellationToken)
        {
            var updateUnpublishedStructuralInformation = _mapper.Map<UnpublishedStructuralInformation>(request);


            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.UpdateUnpublishedStructuralInformation(updateUnpublishedStructuralInformation);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}