using AutoMapper;
using Daikon.Shared.APIClients.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IMLogixAPI _mLogixAPIService;

        public GetHitCollectionByIdQueryHandler(
            IMapper mapper,
            IHitCollectionRepository hitCollectionRepository,
            IHitRepository hitRepository,
            ILogger<GetHitCollectionByIdQueryHandler> logger,
            IMLogixAPI mLogixAPIService)
        {
            _mapper = mapper;
            _hitCollectionRepository = hitCollectionRepository;
            _hitRepository = hitRepository;
            _logger = logger;
            _mLogixAPIService = mLogixAPIService;
        }

        public async Task<HitCollectionVM> Handle(GetHitCollectionByIdQuery request, CancellationToken cancellationToken)
        {

            // Fetch the hit collection
            var hitCollection = await _hitCollectionRepository.ReadHitCollectionById(request.Id);
            if (hitCollection == null)
            {
                _logger.LogWarning("No Hit Collection found for Id: {HitCollectionId}", request.Id);
                return null;
            }

            var hitCollectionViewModel = _mapper.Map<HitCollectionVM>(hitCollection, opts =>
                opts.Items["WithMeta"] = request.WithMeta);

            // Fetch hits for the hit collection
            var hits = await _hitRepository.GetHitsListByHitCollectionId(hitCollection.Id);
            hitCollectionViewModel.Hits = _mapper.Map<List<HitVM>>(hits, opts =>
                opts.Items["WithMeta"] = request.WithMeta);

            foreach (var hit in hitCollectionViewModel.Hits)
            {
                // Assign user's vote if available
                if (hit.Voters.TryGetValue(request.RequestorUserId.ToString(), out var usersVote))
                {
                   // _logger.LogInformation("User {RequestorUserId}'s vote found for HitId {HitId}", request.RequestorUserId, hit.Id);
                    hit.UsersVote = usersVote;
                }
                else
                {
                   // _logger.LogInformation("User {RequestorUserId}'s vote not found for HitId {HitId}", request.RequestorUserId, hit.Id);
                    hit.UsersVote = "NA";
                }

                // Calculate vote score
                hit.VoteScore = CalculateVoteScore(hit);
            }

            // Extract unique MoleculeIds from hits
            var moleculeIds = hitCollectionViewModel.Hits
                .Select(hit => hit.MoleculeId)
                .Distinct()
                .ToList();

            // Fetch and map molecules, if any molecule IDs are available
            if (moleculeIds.Count != 0)
            {
                await FetchAndMapMoleculesAsync(moleculeIds, hitCollectionViewModel, cancellationToken);
            }

            return hitCollectionViewModel;
        }

        private int CalculateVoteScore(HitVM hit)
        {
            // Calculation logic for vote score based on positive, neutral, and negative votes
            return (3 * (int)hit.Positive) + (1 * (int)hit.Neutral) - (3 * (int)hit.Negative);
        }

        private async Task FetchAndMapMoleculesAsync(List<Guid> moleculeIds, HitCollectionVM hitCollectionViewModel, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch all molecules by their IDs in a single call
                var molecules = await _mLogixAPIService.GetMoleculesByIds(moleculeIds);

                if (molecules != null && molecules.Any())
                {
                    var moleculeDictionary = molecules.ToDictionary(m => m.Id);

                    // Map the fetched molecules back to the hits
                    foreach (var hit in hitCollectionViewModel.Hits)
                    {
                        if (moleculeDictionary.TryGetValue(hit.MoleculeId, out var molecule))
                        {
                            hit.Molecule = molecule;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching molecules for Hit Collection Id: {HitCollectionId}", hitCollectionViewModel.Id);
            }
        }
    }
}
