
using AutoMapper;
using Daikon.Shared.APIClients.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;

namespace Screen.Application.Features.Views.GetHitProperties
{
    public class GetHitPropertiesHandler : IRequestHandler<GetHitPropertiesQuery, HitPropertiesVM>
    {
        private readonly IMapper _mapper;
        private readonly IScreenRepository _screenRepository;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IHitRepository _hitRepository;
        private readonly ILogger<GetHitPropertiesHandler> _logger;

        private readonly IMLogixAPI _mLogixAPIService;

        public GetHitPropertiesHandler(IMapper mapper, IScreenRepository screenRepository, 
        IHitCollectionRepository hitCollectionRepository, IHitRepository hitRepository, 
        ILogger<GetHitPropertiesHandler> logger, IMLogixAPI mLogixAPIService)
        {
            _mapper = mapper;
            _screenRepository = screenRepository;
            _hitCollectionRepository = hitCollectionRepository;
            _hitRepository = hitRepository;
            _logger = logger;
            _mLogixAPIService = mLogixAPIService;
        }

        public async Task<HitPropertiesVM> Handle(GetHitPropertiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var hit = await _hitRepository.ReadHitById(request.Id);
                if (hit == null)
                {
                    return new HitPropertiesVM();
                }

                var hitProperties = _mapper.Map<HitPropertiesVM>(hit);

                var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(hit.HitCollectionId);
                if (hitCollection == null)
                {
                    return hitProperties;
                }
                hitProperties.HitCollectionId = hitCollection.Id;

                var screen = await _screenRepository.ReadScreenById(hitCollection.ScreenId);
                if (screen == null)
                {
                    return hitProperties;
                }

                hitProperties.ScreenName = screen.Name;
                hitProperties.ScreenId = hitCollection.ScreenId;

                return hitProperties;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                _logger.LogError(ex, "An error occurred while getting hit properties.");
                throw; // Rethrow the exception to propagate it further
            }
        }


    }

}