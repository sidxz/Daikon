
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

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IGeneRepository _geneRepository;
        private readonly IStrainRepository _strainRepository;


        public UpdateGeneCommandHandler(ILogger<UpdateGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IGeneRepository geneRepository, IStrainRepository strainRepository)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
            _geneRepository = geneRepository;
            _strainRepository = strainRepository;
        }

        public async Task<Unit> Handle(UpdateGeneCommand request, CancellationToken cancellationToken)
        {

            // check if both strainId and strainName are null; reject if they are
            if (request.StrainId == null && request.StrainName == null)
            {
                throw new ArgumentNullException(nameof(request.StrainId), "StrainId and StrainName cannot both be null");
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

            
            var gene = new Domain.Entities.Gene
            {
                Id = request.Id,
                StrainId = strain.Id,
                AccessionNumber = request.AccessionNumber,
                Name = request.Name,
                Function = request.Function,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory
            };

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