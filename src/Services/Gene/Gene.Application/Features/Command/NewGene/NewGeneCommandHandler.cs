using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
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
        private readonly IGeneRepository _geneRepository;

        public NewGeneCommandHandler(ILogger<NewGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository)
        {
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
        }


        public async Task<Unit> Handle(NewGeneCommand request, CancellationToken cancellationToken)
        {
            //var gene = _mapper.Map<Domain.Entities.Gene>(request);

            // check if gene (accessionNo) already exists; reject if it does

            var geneExists = await _geneRepository.ReadGeneByAccession(request.AccessionNumber);
            if (geneExists != null)
            {
                throw new DuplicateEntityRequestException(nameof(NewGeneCommand), request.AccessionNumber);
            }


            var gene = new Domain.Entities.Gene{
                Id = request.Id,
                StrainName = request.StrainName,
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