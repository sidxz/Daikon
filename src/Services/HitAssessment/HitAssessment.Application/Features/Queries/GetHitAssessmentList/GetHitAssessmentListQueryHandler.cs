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
                // Get the list of HitAssessments
                var haS = await _haRepository.GetHaList();
                
                // Map the HitAssessments to ViewModels
                var hasVM = _mapper.Map<List<HitAssessmentListVM>>(haS);

                // Get all HaIds from the list
                var haIds = hasVM.Select(ha => ha.Id).ToList();

                // Fetch all compound evolutions for these HaIds in a single call
                var haCompoundEvolutionsMap = await _haCompoundEvoRepository.GetHaCompoundEvolutionsOfHAs(haIds);

                // Update each HitAssessmentListVM with the latest compound evolution
                foreach (var ha in hasVM)
                {
                    if (haCompoundEvolutionsMap.TryGetValue(ha.Id, out var haCompoundEvolutions))
                    {
                        // Get the latest compound evolution for this HaId
                        var latest = haCompoundEvolutions.LastOrDefault();
                        if (latest != null)
                        {
                            ha.CompoundEvoLatestSMILES = latest.RequestedSMILES;
                            ha.CompoundEvoLatestMoleculeId = latest.MoleculeId;
                        }
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
