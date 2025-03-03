
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using CQRS.Core.Extensions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ByAccession
{
    public class GetGeneByAccessionQueryHandler : IRequestHandler<GetGeneByAccessionQuery, GeneVM>
    {
        private readonly IGeneRepository _geneRepository;
        private readonly IMapper _mapper;
        private readonly IGeneEssentialityRepository _geneEssentialityRepository;
        private readonly IGeneProteinProductionRepository _geneProteinProductionRepository;

        private readonly IGeneProteinActivityAssayRepository _geneProteinActivityAssayRepository;

        private readonly IGeneHypomorphRepository _geneHypomorphRepository;

        private readonly IGeneCrispriStrainRepository _geneCrispriStrainRepository;

        private readonly IGeneResistanceMutationRepository _geneResistanceMutationRepository;

        private readonly IGeneVulnerabilityRepository _geneVulnerabilityRepository;

        private readonly IGeneUnpublishedStructuralInformationRepository _geneUnpublishedStructuralInformationRepository;
        private readonly IGeneExpansionPropRepo _geneExpansionPropRepo;

        public GetGeneByAccessionQueryHandler(IGeneRepository geneRepository, IMapper mapper,
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
        public async Task<GeneVM> Handle(GetGeneByAccessionQuery request, CancellationToken cancellationToken)
        {
            var gene = await _geneRepository.ReadGeneByAccession(request.AccessionNumber);

            if (gene == null)
            {
                throw new ResourceNotFoundException(nameof(Gene), request.AccessionNumber);
            }

            var geneVm = _mapper.Map<GeneVM>(gene, opts => opts.Items["WithMeta"] = request.WithMeta);

            var trackableEntities = new List<VMMeta> { geneVm }; 

            var essentialities = await _geneEssentialityRepository.GetEssentialityOfGene(gene.Id);
            geneVm.Essentialities = _mapper.Map<List<GeneEssentialityVM>>(essentialities, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.Essentialities);

            var proteinProductions = await _geneProteinProductionRepository.GetProteinProductionOfGene(gene.Id);
            geneVm.ProteinProductions = _mapper.Map<List<GeneProteinProductionVM>>(proteinProductions, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.ProteinProductions);

            var proteinActivityAssays = await _geneProteinActivityAssayRepository.GetProteinActivityAssayOfGene(gene.Id);
            geneVm.ProteinActivityAssays = _mapper.Map<List<GeneProteinActivityAssayVM>>(proteinActivityAssays, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.ProteinActivityAssays);

            var hypomorphs = await _geneHypomorphRepository.GetHypomorphOfGene(gene.Id);
            geneVm.Hypomorphs = _mapper.Map<List<GeneHypomorphVM>>(hypomorphs, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.Hypomorphs);

            var crispriStrains = await _geneCrispriStrainRepository.GetCrispriStrainOfGene(gene.Id);
            geneVm.CrispriStrains = _mapper.Map<List<GeneCrispriStrainVM>>(crispriStrains, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.CrispriStrains);

            var resistanceMutations = await _geneResistanceMutationRepository.GetResistanceMutationOfGene(gene.Id);
            geneVm.ResistanceMutations = _mapper.Map<List<GeneResistanceMutationVM>>(resistanceMutations, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.ResistanceMutations);

            var vulnerabilities = await _geneVulnerabilityRepository.GetVulnerabilityOfGene(gene.Id);
            geneVm.Vulnerabilities = _mapper.Map<List<GeneVulnerabilityVM>>(vulnerabilities, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.Vulnerabilities);

            var unpublishedStructuralInformations = await _geneUnpublishedStructuralInformationRepository.GetUnpublishedStructuralInformationOfGene(gene.Id);
            geneVm.UnpublishedStructuralInformations = _mapper.Map<List<GeneUnpublishedStructuralInformationVM>>(unpublishedStructuralInformations, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.UnpublishedStructuralInformations);

            var expansionProps = await _geneExpansionPropRepo.ListByEntityId(gene.Id);
            geneVm.ExpansionProps = _mapper.Map<List<ExpansionPropVM>>(expansionProps, opts => opts.Items["WithMeta"] = request.WithMeta);
            trackableEntities.AddRange(geneVm.ExpansionProps);

            var (pageLastUpdatedDate, pageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);

            // Finally return the complete HA VM
            geneVm.PageLastUpdatedDate = pageLastUpdatedDate;
            geneVm.PageLastUpdatedUser = pageLastUpdatedUser;
            
            return geneVm;
        }
    }
}