
using Amazon.Runtime.Internal.Util;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Infrastructure;
using Screen.Application.Contracts.Persistence;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetHitCollection.ById
{
    public class GetHitCollectionByIdQueryHandler : IRequestHandler<GetHitCollectionByIdQuery, HitCollectionVM>
    {
        private readonly IMapper _mapper;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IHitRepository _hitRepository;
        private readonly ILogger<GetHitCollectionByIdQueryHandler> _logger;

        private readonly IMLogixAPIService _mLogixAPIService;

        public GetHitCollectionByIdQueryHandler(IMapper mapper, IHitCollectionRepository hitCollectionRepository, 
            IHitRepository hitRepository, ILogger<GetHitCollectionByIdQueryHandler> logger, IMLogixAPIService mLogixAPIService)
        {
            _mapper = mapper;
            _hitCollectionRepository = hitCollectionRepository;
            _hitRepository = hitRepository;
            _logger = logger;
            _mLogixAPIService = mLogixAPIService;
        }

        public async Task<HitCollectionVM> Handle(GetHitCollectionByIdQuery request, CancellationToken cancellationToken)
        {
            // fetch hit collection
            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(request.Id);
            var hitCollectionVm = _mapper.Map<HitCollectionVM>(hitCollection, opts => opts.Items["WithMeta"] = request.WithMeta);

            // fetch hits for hit collection
            var hits = await _hitRepository.GetHitsListByHitCollectionId(hitCollection.Id);

            hitCollectionVm.Hits = _mapper.Map<List<HitVM>>(hits, opts => opts.Items["WithMeta"] = request.WithMeta);

            foreach (var hit in hitCollectionVm.Hits)
            {
                try {
                    var molecule = await _mLogixAPIService.GetMoleculeById(hit.MoleculeId);
                   
                    hit.Molecule = _mapper.Map<MoleculeVM>(molecule);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching molecule for hit {HitId}", hit.Id);
                
                }
                
            }

            return hitCollectionVm;
        }
    }
}