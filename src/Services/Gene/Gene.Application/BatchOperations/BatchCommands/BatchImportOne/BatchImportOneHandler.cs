
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Application.Features.Command.NewEssentiality;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.UpdateEssentiality;
using Gene.Application.Features.Command.UpdateGene;
using Gene.Domain.Aggregates;
using Gene.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportOne
{
    public class BatchImportOneCommandHandler : IRequestHandler<BatchImportOneCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<BatchImportOneCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
        private readonly IGeneRepository _geneRepository;
        private readonly IGeneEssentialityRepository _geneEssentialityRepository;
        private readonly IStrainRepository _strainRepository;

        private readonly IMediator _mediator;

        public BatchImportOneCommandHandler(ILogger<BatchImportOneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
                                            IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper,
                                            IMediator mediator, IGeneEssentialityRepository geneEssentialityRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
            _geneEssentialityRepository = geneEssentialityRepository ?? throw new ArgumentNullException(nameof(geneEssentialityRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        public async Task<Unit> Handle(BatchImportOneCommand request, CancellationToken cancellationToken)
        {
            // check if both strainId and strainName are null; reject if they are
            ValidateRequest(request);

            // fetch strain using StrainId or StrainName, whichever is not null

            Strain strain = await FetchStrain(request);

            var existingGene = await _geneRepository.ReadGeneByAccession(request.AccessionNumber);


            if (existingGene == null)
            {
                // create new gene with essentialities
                await NewGene(request, strain.Id);
            }

            else
            {
                // update existing gene
                await UpdateGene(request, existingGene);

            }

            return Unit.Value;

        }



        private void ValidateRequest(BatchImportOneCommand request)
        {
            if (request.StrainId == null && request.StrainName == null)
            {
                throw new ArgumentNullException(nameof(request.StrainId), "Both StrainId and StrainName cannot be null.");
            }
        }


        private async Task<Strain> FetchStrain(BatchImportOneCommand request)
        {
            Strain strain = request.StrainId != null
                ? await _strainRepository.ReadStrainById(request.StrainId.Value)
                : await _strainRepository.ReadStrainByName(request.StrainName);

            if (strain == null)
            {
                throw new ResourceNotFoundException(nameof(StrainAggregate), $"Strain with Id {request.StrainId} or Name {request.StrainName} not found");
            }

            return strain;
        }


        private async Task NewGene(BatchImportOneCommand request, Guid strainId)
        {
            Guid newGeneId = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
            await _mediator.Send(new NewGeneCommand
            {
                Id = newGeneId,
                AccessionNumber = request.AccessionNumber,
                StrainId = strainId,
                Name = request.Name,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory,
            });

            /* Essentiality */

            foreach (var essentiality in request.Essentialities)
            {
                await _mediator.Send(new NewEssentialityCommand
                {
                    Id = newGeneId,
                    GeneId = newGeneId,
                    EssentialityId = essentiality.EssentialityId == Guid.Empty ? Guid.NewGuid() : essentiality.EssentialityId,
                    Classification = essentiality.Classification,
                    Condition = essentiality.Condition,
                    Method = essentiality.Method,
                    Reference = essentiality.Reference,
                    Note = essentiality.Note,
                });
            }
        }


        private async Task UpdateGene(BatchImportOneCommand request, Domain.Entities.Gene existingGene)
        {
            await _mediator.Send(new UpdateGeneCommand
            {
                Id = request.Id,
               
                Name = request.Name,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory,
            });


            /* Essentiality */

            var existingEssentialities = await _geneEssentialityRepository.GetEssentialityOfGene(existingGene.Id);

            // check if essentiality exists
            foreach (var requestedEssentiality in request.Essentialities)
            {
                // check if requestedEssentiality is in existingEssentialities
                var existingEssentiality = existingEssentialities.FirstOrDefault(e => e.EssentialityId == requestedEssentiality.EssentialityId);
                if (existingEssentiality != null)
                {
                    // update it
                    await _mediator.Send(new UpdateEssentialityCommand
                    {
                        Id = existingGene.Id,
                        GeneId = existingGene.Id,
                        EssentialityId = existingEssentiality.EssentialityId,
                        Classification = requestedEssentiality.Classification,
                        Condition = requestedEssentiality.Condition,
                        Method = requestedEssentiality.Method,
                        Reference = requestedEssentiality.Reference,
                        Note = requestedEssentiality.Note,
                    });
                }

                else
                {
                    // create new
                    await _mediator.Send(new NewEssentialityCommand
                    {
                        Id = existingGene.Id,
                        GeneId = existingGene.Id,
                        EssentialityId = requestedEssentiality.EssentialityId == Guid.Empty ? Guid.NewGuid() : requestedEssentiality.EssentialityId,
                        Classification = requestedEssentiality.Classification,
                        Condition = requestedEssentiality.Condition,
                        Method = requestedEssentiality.Method,
                        Reference = requestedEssentiality.Reference,
                        Note = requestedEssentiality.Note,
                    });
                }

            }
        }
    }

}