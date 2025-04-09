
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Application.Features.Command.AddExpansionProp;
using Gene.Application.Features.Command.NewEssentiality;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.UpdateEssentiality;
using Gene.Application.Features.Command.UpdateExpansionProp;
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
        private readonly IGeneExpansionPropRepo _geneExpansionPropRepo;
        private readonly IStrainRepository _strainRepository;

        private readonly IMediator _mediator;

        public BatchImportOneCommandHandler(ILogger<BatchImportOneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
                                            IGeneRepository geneRepository, IStrainRepository strainRepository, IMapper mapper,
                                            IMediator mediator, IGeneExpansionPropRepo geneExpansionPropRepo)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
            _geneExpansionPropRepo = geneExpansionPropRepo ?? throw new ArgumentNullException(nameof(geneExpansionPropRepo));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        public async Task<Unit> Handle(BatchImportOneCommand request, CancellationToken cancellationToken)
        {

            var json = System.Text.Json.JsonSerializer.Serialize(request);
            _logger.LogInformation($"Processing BatchImportOneCommand: {json}");
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
            _logger.LogInformation("Creating new gene");
            request.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
            var newGene = _mapper.Map<NewGeneCommand>(request);
            await _mediator.Send(newGene);

            // create expansion properties
            foreach (var expansionProp in request.ExpansionProps)
            {
               var expansionPropCommand = _mapper.Map<AddExpansionPropCommand>(expansionProp);
               expansionPropCommand.Id = request.Id; // GeneId
               expansionPropCommand.ExpansionPropId = expansionProp.Id == Guid.Empty ? Guid.NewGuid() : expansionProp.Id;
               await _mediator.Send(expansionPropCommand);
            }

        }


        private async Task UpdateGene(BatchImportOneCommand request, Domain.Entities.Gene existingGene)
        {
            
            var updateGene = _mapper.Map<UpdateGeneCommand>(request);
            _logger.LogInformation("Updating existing gene");
            _logger.LogInformation($"Source: {request.Source}");

            if (request.Source == "Mycobrowser")
            {
                // copy over uniprot data
                updateGene.UniProtKB = existingGene.UniProtKB;
                updateGene.ProteinNameExpanded = existingGene.ProteinNameExpanded;
                updateGene.AlphaFoldId = existingGene.AlphaFoldId;

            }
            else if (request.Source == "UniProt")
            {
                // copy over mycobrowser data
                updateGene.Product = existingGene.Product;
                updateGene.FunctionalCategory = existingGene.FunctionalCategory;
                updateGene.Comments = existingGene.Comments;
                updateGene.Coordinates = existingGene.Coordinates;
                updateGene.Orthologues = existingGene.Orthologues;
                updateGene.GeneSequence = existingGene.GeneSequence;
                updateGene.ProteinSequence = existingGene.ProteinSequence;
                updateGene.GeneLength = existingGene.GeneLength;
                updateGene.ProteinLength = existingGene.ProteinLength;

            }
            else {
                // reject if source is neither Mycobrowser nor UniProt
                throw new ArgumentNullException(nameof(request.Source), "Source must be either Mycobrowser or UniProt");
            }
            updateGene.Id = existingGene.Id;
           

            await _mediator.Send(updateGene);

            // update expansion properties if not found, create, else update
            foreach (var expansionProp in request.ExpansionProps)
            {
                var existingExpansionProp = await _geneExpansionPropRepo.FindByTypeAndValue(existingGene.Id, expansionProp.ExpansionType, expansionProp.ExpansionValue.Value);

                if (existingExpansionProp == null)
                {
                    var expansionPropCommand = _mapper.Map<AddExpansionPropCommand>(expansionProp);
                    expansionPropCommand.Id = existingGene.Id; // GeneId
                    expansionPropCommand.ExpansionPropId = expansionProp.Id == Guid.Empty ? Guid.NewGuid() : expansionProp.Id;
                    await _mediator.Send(expansionPropCommand);
                }

                else
                {
                    var updateExpansionPropCommand = _mapper.Map<UpdateExpansionPropCommand>(expansionProp);
                    updateExpansionPropCommand.ExpansionPropId = existingExpansionProp.Id;
                    updateExpansionPropCommand.Id = existingGene.Id; // GeneId
                    await _mediator.Send(updateExpansionPropCommand);
                }
            }
        }
    }

}