using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.NewGene
{
    public class NewGeneCommandHandler : IRequestHandler<NewGeneCommand, Unit>
    {

        //private readonly IMapper _mapper;
        private readonly ILogger<NewGeneCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

        public NewGeneCommandHandler(ILogger<NewGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
        {
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
        }


        public async Task<Unit> Handle(NewGeneCommand request, CancellationToken cancellationToken)
        {
            //var gene = _mapper.Map<Domain.Entities.Gene>(request);
            var gene = new Domain.Entities.Gene{
                Id = request.Id,
                AccessionNumber = request.AccessionNumber,
                Name = request.Name,
                Function = request.Function,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory
            };
            
            var aggregate = new GeneAggregate(gene);
            await _eventSourcingHandler.SaveAsync(aggregate);

            return Unit.Value;
            
        }
    }

}