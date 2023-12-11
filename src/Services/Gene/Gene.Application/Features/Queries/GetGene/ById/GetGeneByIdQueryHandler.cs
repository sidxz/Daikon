
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
        private readonly IMapper _mapper;


        public GetGeneByIdQueryHandler(IGeneRepository geneRepository, IMapper mapper, IGeneEssentialityRepository geneEssentialityRepository)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _geneEssentialityRepository = geneEssentialityRepository ?? throw new ArgumentNullException(nameof(geneEssentialityRepository));
        }
        public async Task<GeneVM> Handle(GetGeneByIdQuery request, CancellationToken cancellationToken)
        {
            
            var gene = await _geneRepository.ReadGeneById(request.Id);

            if (gene == null)
            {
                throw new ResourceNotFoundException(nameof(Gene), request.Id);
            }

            var essentialities = await _geneEssentialityRepository.GetEssentialityOfGene(gene.Id);

            var geneVm = _mapper.Map<GeneVM>(gene, opts => opts.Items["WithMeta"] = request.WithMeta);
            geneVm.Essentialities = _mapper.Map<List<GeneEssentialityVM>>(essentialities, opts => opts.Items["WithMeta"] = request.WithMeta);
            
            return geneVm;

        }
    }
}