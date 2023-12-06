
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewStrain
{
    public class NewStrainCommandHandler : IRequestHandler<NewStrainCommand, Unit>
    {

        //private readonly IMapper _mapper;
        private readonly ILogger<NewStrainCommandHandler> _logger;

        private readonly IEventSourcingHandler<StrainAggregate> _eventSourcingHandler;
        private readonly IStrainRepository _strainRepository;

        public NewStrainCommandHandler(ILogger<NewStrainCommandHandler> logger, IEventSourcingHandler<StrainAggregate> eventSourcingHandler, IStrainRepository strainRepository)
        {
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
        }


        public async Task<Unit> Handle(NewStrainCommand request, CancellationToken cancellationToken)
        {
            //var gene = _mapper.Map<Domain.Entities.Gene>(request);

            // check if strain (name) already exists; reject if it does

            var strainExists = await _strainRepository.ReadStrainByName(request.Name);
            if (strainExists != null)
            {
                throw new DuplicateEntityRequestException(nameof(NewStrainCommand), request.Name);
            }


            var strain = new Strain{
                Id = request.Id,
                Name = request.Name,
                Organism = request.Organism,
                
            };
            
            var aggregate = new StrainAggregate(strain);
            await _eventSourcingHandler.SaveAsync(aggregate);

            return Unit.Value;
            
        }
    }

}