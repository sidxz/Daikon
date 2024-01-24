
using AutoMapper;
using Target.Application.Contracts.Persistence;
using MediatR;
using CQRS.Core.Exceptions;

namespace Target.Application.Features.Queries.GetTargetsList
{
    public class GetTargetsListQueryHandler : IRequestHandler<GetTargetsListQuery, List<TargetsListVM>>
    {
        private readonly ITargetRepository _targetRepository;
        private readonly IMapper _mapper;


        public GetTargetsListQueryHandler(ITargetRepository targetRepository, IMapper mapper)
        {
            _targetRepository = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }
        public async Task<List<TargetsListVM>> Handle(GetTargetsListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var targets = await _targetRepository.GetTargetsList();
                
                return _mapper.Map<List<TargetsListVM>>(targets);
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Gene Repository", ex);
            }


        }
    }
}