
using AutoMapper;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetGenesList
{
    public class GetGenesListQueryHandler : IRequestHandler<GetGenesListQuery, List<GenesListVM>>
    {
        private readonly IGeneRepository _geneRepository;
        private readonly IMapper _mapper;

        public GetGenesListQueryHandler(IGeneRepository geneRepository, IMapper mapper)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<GenesListVM>> Handle(GetGenesListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var geneList = await _geneRepository.GetGenesList();
                
                return _mapper.Map<List<GenesListVM>>(geneList);
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Gene Repository", ex);
            }
        }
    }
}