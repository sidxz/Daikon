
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Strains;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewStrain
{
    public class NewStrainCommandHandler : IRequestHandler<NewStrainCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewStrainCommandHandler> _logger;
        private readonly IEventSourcingHandler<StrainAggregate> _eventSourcingHandler;
        private readonly IStrainRepository _strainRepository;

        public NewStrainCommandHandler(ILogger<NewStrainCommandHandler> logger,
                IEventSourcingHandler<StrainAggregate> eventSourcingHandler,
                IStrainRepository strainRepository,
                IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
        }


        public async Task<Unit> Handle(NewStrainCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewStrainCommandHandler {request}", request);

            request.SetCreateProperties(request.RequestorUserId);

            var strainExists = await _strainRepository.ReadStrainByName(request.Name);
            if (strainExists != null)
            {
                throw new DuplicateEntityRequestException(nameof(NewStrainCommand), request.Name);
            }


            var strainCreatedEvent = _mapper.Map<StrainCreatedEvent>(request);
            strainCreatedEvent.CreatedById = request.RequestorUserId;
            try
            {
                var aggregate = new StrainAggregate(strainCreatedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling NewGeneCommand");
                throw;
            }
        }
    }

}