
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Target.Application.Contracts.Persistence;
using Target.Application.Features.Queries.CommonVMs;

namespace Target.Application.Features.Queries.GetToxicology.ByTargetId
{
    public class GetToxicologyByTargetHandler(IToxicologyRepo toxicologyRepo, IMapper mapper) : IRequestHandler<GetToxicologyByTargetQuery, List<ToxicologyVM>>
    {
        private readonly IToxicologyRepo _toxicologyRepo = toxicologyRepo ?? throw new ArgumentNullException(nameof(toxicologyRepo));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<List<ToxicologyVM>> Handle(GetToxicologyByTargetQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var toxicologies = await _toxicologyRepo.ReadByTargetId(request.TargetId);

                var toxicologiesVM = _mapper.Map<List<ToxicologyVM>>(toxicologies, opts => opts.Items["WithMeta"] = request.WithMeta);


                return toxicologiesVM;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Toxicology Repository", ex);
            }
        }
    }
}