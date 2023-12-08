
using AutoMapper;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ById
{
    public class GetGeneByIdQueryHandler : IRequestHandler<GetGeneByIdQuery, GeneVM>
    {
        private readonly IGeneRepository _geneRepository;
        private readonly IMapper _mapper;


        public GetGeneByIdQueryHandler(IGeneRepository geneRepository, IMapper mapper)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<GeneVM> Handle(GetGeneByIdQuery request, CancellationToken cancellationToken)
        {
            
            var gene = await _geneRepository.ReadGeneById(request.Id);

            if (gene == null)
            {
                throw new ResourceNotFoundException(nameof(Gene), request.Id);
            }

            return _mapper.Map<GeneVM>(gene, opts => opts.Items["WithMeta"] = request.WithMeta);

        }
    }
}