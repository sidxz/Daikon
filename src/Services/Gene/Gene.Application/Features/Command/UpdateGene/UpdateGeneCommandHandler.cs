
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.UpdateGene
{
    public class UpdateGeneCommandHandler : IRequestHandler<UpdateGeneCommand, Unit>
    {

        private readonly ILogger<UpdateGeneCommandHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IGeneRepository _geneRepository;
        private readonly IStrainRepository _strainRepository;


        public UpdateGeneCommandHandler(ILogger<UpdateGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
                                        IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _geneRepository = geneRepository;
            _strainRepository = strainRepository;
        }

        public async Task<Unit> Handle(UpdateGeneCommand request, CancellationToken cancellationToken)
        {

            // check if both strainId and strainName are null; reject if they are
            if (request.StrainId == null && request.StrainName == null)
            {
                throw new ArgumentNullException(nameof(request.StrainId), "Both StrainId and StrainName cannot be null.");
            }

            // check if accessionNo is modified; reject if it is
            var existingGene = await _geneRepository.ReadGeneById(request.Id);
            if (existingGene.AccessionNumber != request.AccessionNumber)
            {
                throw new InvalidOperationException("AccessionNumber cannot be modified");
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

            // Things that cannot be modified
            gene.AccessionNumber = existingGene.AccessionNumber;
            gene.StrainId = strain.Id;
            
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateGene(gene);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(GeneAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}