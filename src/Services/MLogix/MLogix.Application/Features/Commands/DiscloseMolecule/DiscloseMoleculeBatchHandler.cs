
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Domain.Aggregates;
using MLogix.Application.Features.Commands.PredictNuisance;
using MLogix.Application.DTOs.CageFusion;
using MLogix.Application.BackgroundServices;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using MLogix.Application.Utils;

namespace MLogix.Application.Features.Commands.DiscloseMolecule
{
    public class DiscloseMoleculeBatchHandler : IRequestHandler<DiscloseMoleculeBatchCommand, IList<MoleculeVM>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DiscloseMoleculeBatchHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        private readonly INuisanceJobQueue _nuisanceQueue;


        public DiscloseMoleculeBatchHandler(
            IMapper mapper,
            ILogger<DiscloseMoleculeBatchHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor,
            INuisanceJobQueue nuisanceQueue,
            IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _nuisanceQueue = nuisanceQueue ?? throw new ArgumentNullException(nameof(nuisanceQueue));
        }

        public async Task<IList<MoleculeVM>> Handle(DiscloseMoleculeBatchCommand request, CancellationToken cancellationToken)
        {
            if (request.Molecules == null || request.Molecules.Count == 0)
                throw new ArgumentException("At least one molecule disclosure request must be provided.");

            var results = new List<MoleculeVM>();
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                          ?? new Dictionary<string, string>();


            var candidates = request.Molecules
                .Where(m =>
                    m.Id != Guid.Empty &&
                    !string.IsNullOrWhiteSpace(m.Name) &&
                    !string.IsNullOrWhiteSpace(m.RequestedSMILES))
                .ToList();

            if (candidates.Count == 0)
                return [];

            // dedupe by Id
            candidates = candidates
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            // 2) Batch fetch local molecules
            var ids = candidates.Select(c => c.Id).Distinct().ToList();
            var localMolecules = await _moleculeRepository.GetMoleculesByIds(ids);

            var localById = localMolecules.ToDictionary(m => m.Id, m => m);

            // Remove missing + already disclosed
            candidates.RemoveAll(c =>
            {
                if (!localById.TryGetValue(c.Id, out var local))
                    return true;

                // already disclosed locally
                return !string.IsNullOrWhiteSpace(local.RequestedSMILES);
            });


            if (candidates.Count == 0)
                return [];

            var smilesToCheck = candidates
                .Select(c => c.RequestedSMILES.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();

            var vaultMatches = await _moleculeAPI.GetMoleculesBySMILES(smilesToCheck, headers);
            var vaultRegIds = vaultMatches.Select(v => v.Id).ToHashSet();

            candidates.RemoveAll(c =>
            {
                if (!localById.TryGetValue(c.Id, out var local)) return true;
                return vaultRegIds.Contains(local.RegistrationId);
            });


            var registrationCommands = candidates.Select(c =>
                {
                    var local = localById[c.Id];

                    return new RegisterMoleculeCommandWithRegId
                    {
                        Id = local.RegistrationId,
                        Name = local.Name,
                        SMILES = c.RequestedSMILES,
                        RequestorUserId = c.RequestorUserId
                    };
                }).ToList();

            List<MoleculeBase> registered;
            try
            {
                registered = await _moleculeAPI.RegisterBatch(registrationCommands, previewMode: request.PreviewMode, headers: headers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed ChemVault batch registration.");
                return new List<MoleculeVM>();
            }


            var registeredByRegId = registered.ToDictionary(r => r.Id, r => r);

            // 5) Event sourcing + results
            var nuisanceList = new List<NuisanceRequestTuple>(capacity: registered.Count);

            headers.TryGetValue("AppUser-FullName", out var scientistFromHeader);
            Guid orgIdFromHeader = Guid.Empty;
            if (headers.TryGetValue("AppOrg-Id", out var orgStr))
                Guid.TryParse(orgStr, out orgIdFromHeader);

            var utcNow = DateTime.UtcNow;
            // Find out candidates that were successfully registered in ChemVault but have a different RegId than local
            // TEST CODE
            foreach (var candidate in candidates)
            {
                var local = localById[candidate.Id];
                if (registeredByRegId.TryGetValue(local.RegistrationId, out _))
                    continue; // RegId matches, nothing to do

                // Check if candidates name matches with any registered molecules names and synonyms

                var vaultMol = registered.FirstOrDefault(r =>
                    string.Equals(r.Name, candidate.Name, StringComparison.OrdinalIgnoreCase) ||
                    (r.Synonyms != null && StringUtilities.ExtractSynonyms(r.Synonyms)
                        .Any(syn => string.Equals(syn, candidate.Name, StringComparison.OrdinalIgnoreCase))));

                if (vaultMol != null)
                {
                    _logger.LogWarning(
                        "MLogix Molecule {Name} Id: {Id} RegId: {RegId},  SMILES already registered in ChemVault with a different Molecule. ChemVault: {CvName}, RegId: {CvRegId}",
                        local.Name, local.Id, local.RegistrationId,
                        vaultMol.Name, vaultMol.Id);
                

                    // This is where merger should happen
                    
                }
            } 

             // END TEST CODE

            foreach (var candidate in candidates)
            {
                var local = localById[candidate.Id];
                if (!registeredByRegId.TryGetValue(local.RegistrationId, out var vaultMol))
                    continue; // ChemVault might have partial failures; skip those safely.

                var disclosedEvent = _mapper.Map<MoleculeDisclosedEvent>(candidate);
                disclosedEvent.Id = local.Id;
                disclosedEvent.RegistrationId = local.RegistrationId;
                disclosedEvent.RequestedSMILES = candidate.RequestedSMILES;
                disclosedEvent.SmilesCanonical = vaultMol.SmilesCanonical;

                disclosedEvent.StructureDisclosedDate = utcNow;
                disclosedEvent.StructureDisclosedByUserId = request.RequestorUserId;
                disclosedEvent.IsStructureDisclosed = true;

                disclosedEvent.DisclosureScientist = !string.IsNullOrWhiteSpace(candidate.DisclosureScientist)
                    ? candidate.DisclosureScientist
                    : (scientistFromHeader ?? string.Empty);

                disclosedEvent.DisclosureOrgId = candidate.DisclosureOrgId != Guid.Empty
                    ? candidate.DisclosureOrgId
                    : orgIdFromHeader;


                if (request.PreviewMode)
                {
                    // In preview mode, we do not persist changes, so skip to next
                    var vmPreview = _mapper.Map<MoleculeVM>(vaultMol);
                    vmPreview.Id = local.Id;
                    vmPreview.RegistrationId = local.RegistrationId;
                    results.Add(vmPreview);
                    _logger.LogInformation("Preview mode - Should Disclose: {Id}", local.Id);
                    continue;
                }


                try
                {
                    var aggregate = await _moleculeEventSourcingHandler.GetByAsyncId(local.Id);
                    aggregate.DiscloseMolecule(disclosedEvent);
                    await _moleculeEventSourcingHandler.SaveAsync(aggregate);
                }
                catch (AggregateNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Aggregate not found for molecule: {Id}", local.Id);
                    continue;
                }


                var vm = _mapper.Map<MoleculeVM>(vaultMol);
                vm.Id = local.Id;
                vm.RegistrationId = local.RegistrationId;
                results.Add(vm);
                if (!request.SkipNuisanceCheck)
                {
                    nuisanceList.Add(new NuisanceRequestTuple
                    {
                        Id = local.Id.ToString(),
                        SMILES = disclosedEvent.SmilesCanonical
                    });
                }

            }
            if (!request.SkipNuisanceCheck)
                await EnqueueNuisanceAsync(nuisanceList, request, utcNow, cancellationToken);
            _logger.LogInformation("Disclosed molecules count: {Count}", results.Count);
            return results;

        }


        private async Task EnqueueNuisanceAsync(
                        List<NuisanceRequestTuple> nuisanceList,
                        DiscloseMoleculeBatchCommand request,
                        DateTime utcNow,
                        CancellationToken cancellationToken)
        {
            if (nuisanceList.Count == 0) return;

            var nuisanceCommand = new PredictNuisanceCommand
            {
                NuisanceRequestTuple = nuisanceList,
                PlotAllAttention = false,
                RequestorUserId = request.RequestorUserId,
                CreatedById = request.CreatedById,
                DateCreated = utcNow,
                IsModified = false,
                LastModifiedById = request.LastModifiedById,
                DateModified = utcNow,
            };

            try
            {
                var correlationId = Guid.NewGuid().ToString("N");
                using (_logger.BeginScope(new Dictionary<string, object> { ["corrId"] = correlationId }))
                {
                    var job = new NuisanceJob(nuisanceCommand, correlationId);
                    await _nuisanceQueue.EnqueueAsync(job, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to trigger nuisance predictions.");
            }
        }

    }
}
