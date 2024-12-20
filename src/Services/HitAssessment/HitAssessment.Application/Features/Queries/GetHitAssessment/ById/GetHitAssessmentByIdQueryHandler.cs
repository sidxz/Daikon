
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using CQRS.Core.Extensions;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.VM.MLogix;
using HitAssessment.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment.ById
{
    public class GetHitAssessmentByIdQueryHandler : IRequestHandler<GetHitAssessmentByIdQuery, HitAssessmentVM>
    {
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;
        private readonly IMapper _mapper;
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly ILogger<GetHitAssessmentByIdQueryHandler> _logger;

        public GetHitAssessmentByIdQueryHandler(IHitAssessmentRepository haRepository,
        IMapper mapper, IHaCompoundEvolutionRepository haCompoundEvoRepository, IMLogixAPI mLogixAPIService,
        ILogger<GetHitAssessmentByIdQueryHandler> logger)
        {
            _haRepository = haRepository ?? throw new ArgumentNullException(nameof(haRepository));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<HitAssessmentVM> Handle(GetHitAssessmentByIdQuery request, CancellationToken cancellationToken)
        {

            // Fetch HA from db
            var ha = await _haRepository.ReadHaById(request.Id);

            if (ha == null)
            {
                throw new ResourceNotFoundException(nameof(HitAssessment), request.Id);
            }

            // map to ha VM
            var haVm = _mapper.Map<HitAssessmentVM>(ha, opts => opts.Items["WithMeta"] = request.WithMeta);

            var trackableEntities = new List<VMMeta> { haVm }; 

            // fetch compound evolution of HA. This returns a list of compound evolutions
            var haCompoundEvo = await _haCompoundEvoRepository.GetHaCompoundEvolutionOfHa(request.Id);

            if (haCompoundEvo.Count >= 1)
            {
                // map each compound evolution to compound evolution VM
                haVm.HaCompoundEvolution = _mapper.Map<List<HaCompoundEvolutionVM>>(haCompoundEvo, opts => opts.Items["WithMeta"] = request.WithMeta);
                haVm.CompoundEvoLatestSMILES = (string)haVm.HaCompoundEvolution.LastOrDefault()?.RequestedSMILES;
                haVm.CompoundEvoLatestMoleculeId = (Guid)haVm.HaCompoundEvolution.LastOrDefault()?.MoleculeId;


                // fetch molecule for each compound evolution
                foreach (var evolution in haVm.HaCompoundEvolution)
                {
                    try
                    {
                        var molecule = await _mLogixAPIService.GetMoleculeById(evolution.MoleculeId);

                        evolution.Molecule = _mapper.Map<MoleculeVM>(molecule);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching molecule for compound evolution with Molecule Id", evolution.MoleculeId);

                    }
                }
            }
            else
            {
                haVm.HaCompoundEvolution = [];
            }

            trackableEntities.AddRange(haVm.HaCompoundEvolution);
            (haVm.PageLastUpdatedDate, haVm.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);
            // Finally return the complete HA VM
            return haVm;

        }
    }
}