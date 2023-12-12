
using AutoMapper;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ById
{
    public class GetGeneByIdQueryHandler : IRequestHandler<GetGeneByIdQuery, GeneVM>
    {
        private readonly IGeneRepository _geneRepository;
        private readonly IGeneEssentialityRepository _geneEssentialityRepository;

        private readonly IGeneProteinProductionRepository _geneProteinProductionRepository;

        private readonly IGeneProteinActivityAssayRepository _geneProteinActivityAssayRepository;




        private readonly IMapper _mapper;


        public GetGeneByIdQueryHandler(IGeneRepository geneRepository, IMapper mapper, 
                                IGeneEssentialityRepository geneEssentialityRepository,
                                IGeneProteinProductionRepository geneProteinProductionRepository,
                                IGeneProteinActivityAssayRepository geneProteinActivityAssayRepository
                                )
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _geneEssentialityRepository = geneEssentialityRepository ?? throw new ArgumentNullException(nameof(geneEssentialityRepository));
            _geneProteinProductionRepository = geneProteinProductionRepository ?? throw new ArgumentNullException(nameof(geneProteinProductionRepository));
            _geneProteinActivityAssayRepository = geneProteinActivityAssayRepository ?? throw new ArgumentNullException(nameof(geneProteinActivityAssayRepository));
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

            return geneVm;

        }
    }
}