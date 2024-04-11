
using AutoMapper;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ById
{
    public class GetGeneByIdQueryHandler : IRequestHandler<GetGeneByIdQuery, GeneVM>
    {
        private readonly IGeneRepository _geneRepository;
        private readonly IGeneEssentialityRepository _geneEssentialityRepository;
        private readonly IGeneProteinProductionRepository _geneProteinProductionRepository;
        private readonly IGeneProteinActivityAssayRepository _geneProteinActivityAssayRepository;
        private readonly IGeneHypomorphRepository _geneHypomorphRepository;
        private readonly IGeneCrispriStrainRepository _geneCrispriStrainRepository;
        private readonly IGeneResistanceMutationRepository _geneResistanceMutationRepository;
        private readonly IGeneVulnerabilityRepository _geneVulnerabilityRepository;
        private readonly IGeneUnpublishedStructuralInformationRepository _geneUnpublishedStructuralInformationRepository;
        private readonly IGeneExpansionPropRepo _geneExpansionPropRepo;
        private readonly IMapper _mapper;


        public GetGeneByIdQueryHandler(IGeneRepository geneRepository, IMapper mapper,
                                IGeneEssentialityRepository geneEssentialityRepository,
                                IGeneProteinProductionRepository geneProteinProductionRepository,
                                IGeneProteinActivityAssayRepository geneProteinActivityAssayRepository,
                                IGeneHypomorphRepository geneHypomorphRepository,
                                IGeneCrispriStrainRepository geneCrispriStrainRepository,
                                IGeneResistanceMutationRepository geneResistanceMutationRepository,
                                IGeneVulnerabilityRepository geneVulnerabilityRepository,
                                IGeneUnpublishedStructuralInformationRepository geneUnpublishedStructuralInformationRepository,
                                IGeneExpansionPropRepo geneExpansionPropRepo

                                )
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _geneEssentialityRepository = geneEssentialityRepository ?? throw new ArgumentNullException(nameof(geneEssentialityRepository));
            _geneProteinProductionRepository = geneProteinProductionRepository ?? throw new ArgumentNullException(nameof(geneProteinProductionRepository));
            _geneProteinActivityAssayRepository = geneProteinActivityAssayRepository ?? throw new ArgumentNullException(nameof(geneProteinActivityAssayRepository));
            _geneHypomorphRepository = geneHypomorphRepository ?? throw new ArgumentNullException(nameof(geneHypomorphRepository));
            _geneCrispriStrainRepository = geneCrispriStrainRepository ?? throw new ArgumentNullException(nameof(geneCrispriStrainRepository));
            _geneResistanceMutationRepository = geneResistanceMutationRepository ?? throw new ArgumentNullException(nameof(geneResistanceMutationRepository));
            _geneVulnerabilityRepository = geneVulnerabilityRepository ?? throw new ArgumentNullException(nameof(geneVulnerabilityRepository));
            _geneUnpublishedStructuralInformationRepository = geneUnpublishedStructuralInformationRepository ?? throw new ArgumentNullException(nameof(geneUnpublishedStructuralInformationRepository));
            _geneExpansionPropRepo = geneExpansionPropRepo ?? throw new ArgumentNullException(nameof(geneExpansionPropRepo));
        }
        public async Task<GeneVM> Handle(GetGeneByIdQuery request, CancellationToken cancellationToken)
        {

            var gene = await _geneRepository.ReadGeneById(request.Id);

            if (gene == null)
            {
                throw new ResourceNotFoundException(nameof(Gene), request.Id);
            }

            var geneVm = _mapper.Map<GeneVM>(gene, opts => opts.Items["WithMeta"] = request.WithMeta);

            var essentialities = await _geneEssentialityRepository.GetEssentialityOfGene(gene.Id);
            geneVm.Essentialities = _mapper.Map<List<GeneEssentialityVM>>(essentialities, opts => opts.Items["WithMeta"] = request.WithMeta);

            var proteinProductions = await _geneProteinProductionRepository.GetProteinProductionOfGene(gene.Id);
            geneVm.ProteinProductions = _mapper.Map<List<GeneProteinProductionVM>>(proteinProductions, opts => opts.Items["WithMeta"] = request.WithMeta);

            var proteinActivityAssays = await _geneProteinActivityAssayRepository.GetProteinActivityAssayOfGene(gene.Id);
            geneVm.ProteinActivityAssays = _mapper.Map<List<GeneProteinActivityAssayVM>>(proteinActivityAssays, opts => opts.Items["WithMeta"] = request.WithMeta);

            var hypomorphs = await _geneHypomorphRepository.GetHypomorphOfGene(gene.Id);
            geneVm.Hypomorphs = _mapper.Map<List<GeneHypomorphVM>>(hypomorphs, opts => opts.Items["WithMeta"] = request.WithMeta);

            var crispriStrains = await _geneCrispriStrainRepository.GetCrispriStrainOfGene(gene.Id);
            geneVm.CrispriStrains = _mapper.Map<List<GeneCrispriStrainVM>>(crispriStrains, opts => opts.Items["WithMeta"] = request.WithMeta);

            var resistanceMutations = await _geneResistanceMutationRepository.GetResistanceMutationOfGene(gene.Id);
            geneVm.ResistanceMutations = _mapper.Map<List<GeneResistanceMutationVM>>(resistanceMutations, opts => opts.Items["WithMeta"] = request.WithMeta);

            var vulnerabilities = await _geneVulnerabilityRepository.GetVulnerabilityOfGene(gene.Id);
            geneVm.Vulnerabilities = _mapper.Map<List<GeneVulnerabilityVM>>(vulnerabilities, opts => opts.Items["WithMeta"] = request.WithMeta);

            var unpublishedStructuralInformations = await _geneUnpublishedStructuralInformationRepository.GetUnpublishedStructuralInformationOfGene(gene.Id);
            geneVm.UnpublishedStructuralInformations = _mapper.Map<List<GeneUnpublishedStructuralInformationVM>>(unpublishedStructuralInformations, opts => opts.Items["WithMeta"] = request.WithMeta);

            var expansionProps = await _geneExpansionPropRepo.ListByEntityId(gene.Id);
            geneVm.ExpansionProps = _mapper.Map<List<ExpansionPropVM>>(expansionProps, opts => opts.Items["WithMeta"] = request.WithMeta);

            return geneVm;

        }
    }
}