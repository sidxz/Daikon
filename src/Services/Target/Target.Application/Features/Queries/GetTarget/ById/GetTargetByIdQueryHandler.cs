
using AutoMapper;
using CQRS.Core.Exceptions;
using Target.Application.Contracts.Persistence;
using MediatR;
using CQRS.Core.Domain;
using CQRS.Core.Extensions;

namespace Target.Application.Features.Queries.GetTarget.ById
{
    public class GetTargetByIdQueryHandler : IRequestHandler<GetTargetByIdQuery, TargetVM>
    {
        private readonly ITargetRepository _targetRepository;
        private readonly IMapper _mapper;


        public GetTargetByIdQueryHandler(ITargetRepository targetRepository, IMapper mapper)
        {
            _targetRepository = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public async Task<TargetVM> Handle(GetTargetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var target = await _targetRepository.ReadTargetById(request.Id) ?? throw new ResourceNotFoundException(nameof(Target), request.Id);


                var targetVm = _mapper.Map<TargetVM>(target, opts => opts.Items["WithMeta"] = request.WithMeta);
                targetVm.AssociatedGenesFlattened = string.Join(", ", targetVm.AssociatedGenes.Select(x => x.Value));

                var trackableEntities = new List<VMMeta> { targetVm }; 
                (targetVm.PageLastUpdatedDate, targetVm.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);

                return targetVm;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Target Repository", ex);
            }

        }
    }
}