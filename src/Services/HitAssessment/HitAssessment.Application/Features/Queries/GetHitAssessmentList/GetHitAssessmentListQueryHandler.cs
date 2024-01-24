
using AutoMapper;
using HitAssessment.Application.Contracts.Persistence;
using MediatR;
using CQRS.Core.Exceptions;
using HitAssessment.Application.Features.Queries.GetHitAssessmentList;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment.GetHitAssessmentList
{
    public class GetHitAssessmentListQueryHandler : IRequestHandler<GetHitAssessmentListQuery, List<HitAssessmentListVM>>
    {
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IMapper _mapper;


        public GetHitAssessmentListQueryHandler(IHitAssessmentRepository targetRepository, IMapper mapper)
        {
            _haRepository = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public async Task<List<HitAssessmentListVM>> Handle(GetHitAssessmentListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var haS = await _haRepository.GetHaList();

                return _mapper.Map<List<HitAssessmentListVM>>(haS);
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in HA Repository", ex);
            }


        }
    }
}