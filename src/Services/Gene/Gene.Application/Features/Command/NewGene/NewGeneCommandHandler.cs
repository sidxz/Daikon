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

        private readonly IMapper _mapper;
        private readonly ILogger<NewGeneCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IGeneRepository _geneRepository;
        private readonly IStrainRepository _strainRepository;

        public NewGeneCommandHandler(ILogger<NewGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
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

            // check if both strainId and strainName are null; reject if they are
            if (request.StrainId == null && request.StrainName == null)
            {
                throw new ArgumentNullException(nameof(request.StrainId), "Both StrainId and StrainName cannot be null.");
            }

            // fetch strain using StrainId or StrainName, whichever is not null
            Strain strain;
            if (request.StrainId != null)
            {
                strain = await _strainRepository.ReadStrainById(request.StrainId.Value);
            }
            else
            {
                strain = await _strainRepository.ReadStrainByName(request.StrainName);
            }

            // reject if strain is null
            if (strain == null)
            {
                throw new ResourceNotFoundException(nameof(StrainAggregate), $"Strain with Id {request.StrainId} or Name {request.StrainName} not found");
            }

            var gene = _mapper.Map<Domain.Entities.Gene>(request);


            // var gene = new Domain.Entities.Gene
            // {
            //     Id = request.Id,
            //     StrainId = strain.Id,
            //     AccessionNumber = request.AccessionNumber,
            //     Name = request.Name,
            //     Function = request.Function,
            //     Product = request.Product,
            //     FunctionalCategory = request.FunctionalCategory
            // };

            var aggregate = new GeneAggregate(gene);
            await _eventSourcingHandler.SaveAsync(aggregate);

            return Unit.Value;

        }
    }

}