
using AutoMapper;
using CQRS.Core.Exceptions;
using HitAssessment.Application.Contracts.Persistence;
using MediatR;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment.ById
{
    public class GetHitAssessmentByIdQueryHandler : IRequestHandler<GetHitAssessmentByIdQuery, HitAssessmentVM>
    {
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IMapper _mapper;


        public GetHitAssessmentByIdQueryHandler(IHitAssessmentRepository haRepository, IMapper mapper)
        {
            _haRepository = haRepository ?? throw new ArgumentNullException(nameof(haRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }
        public async Task<HitAssessmentVM> Handle(GetHitAssessmentByIdQuery request, CancellationToken cancellationToken)
        {
            
            var ha = await _haRepository.ReadHaById(request.Id);

            if (ha == null)
            {
                throw new ResourceNotFoundException(nameof(HitAssessment), request.Id);
            }

            var haVm = _mapper.Map<HitAssessmentVM>(ha, opts => opts.Items["WithMeta"] = request.WithMeta);

            return haVm;

        }
    }
}