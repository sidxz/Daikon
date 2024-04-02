
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
                var targetsListVm = _mapper.Map<List<TargetsListVM>>(targets, opts => opts.Items["WithMeta"] = request.WithMeta);

                // Flatten the gene dictionary to a string
                foreach (var target in targetsListVm)
                {
                    target.AssociatedGenesFlattened = string.Join(", ", target.AssociatedGenes.Select(x => x.Value));
                }
                return targetsListVm;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Gene Repository", ex);
            }


        }
    }
}