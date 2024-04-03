
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
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;
        private readonly IMapper _mapper;


        public GetHitAssessmentListQueryHandler(IHitAssessmentRepository targetRepository, IMapper mapper, IHaCompoundEvolutionRepository haCompoundEvoRepository)
        {
            _haRepository = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public async Task<List<HitAssessmentListVM>> Handle(GetHitAssessmentListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var haS = await _haRepository.GetHaList();
                var hasVM = _mapper.Map<List<HitAssessmentListVM>>(haS);
                foreach (var ha in hasVM)
                {
                    // fetch compound evolution of HA. This returns a list of compound evolutions
                    var haCompoundEvo = await _haCompoundEvoRepository.GetHaCompoundEvolutionOfHa(ha.Id);
                    var latest = haCompoundEvo.FirstOrDefault();
                    if (latest != null)
                    {
                        ha.CompoundEvoLatestSMILES = latest.RequestedSMILES;
                    }
                }
                return hasVM;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in HA Repository", ex);
            }


        }
    }
}