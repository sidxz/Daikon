using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.DTO.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Domain.Aggregates;
using CQRS.Core.Extensions;
using Screen.Domain.Entities;
using Daikon.Shared.VM.Screen;
using Daikon.Shared.VM.MLogix;

namespace Screen.Application.Features.Commands.NewHitBatch
{
    /*
     * Handles the registration of a batch of new hits to the system.
     * This includes molecule registration, event generation, and aggregate persistence.
     */
    public class RegisterHitBatchHandler : IRequestHandler<RegisterHitBatchCommand, List<HitVM>>
    {
        public const int MaxBatchSize = 500;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterHitBatchHandler> _logger;
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;

        public RegisterHitBatchHandler(
            IMapper mapper,
            ILogger<RegisterHitBatchHandler> logger,
            IMLogixAPI mLogixAPIService,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
        }

        public async Task<List<HitVM>> Handle(RegisterHitBatchCommand request, CancellationToken cancellationToken)
        {
            var batches = request.Commands.Batch(500); // In case of large input
            var addedHitsResults = new List<HitVM>();

            foreach (var batch in batches)
            {
                // Step 1: Set creation metadata and fallback HitId
                foreach (var cmd in batch)
                {
                    cmd.RequestorUserId = request.RequestorUserId;
                    cmd.SetCreateProperties(request.RequestorUserId);
                    cmd.HitId = (cmd.HitId ?? Guid.Empty) == Guid.Empty ? Guid.NewGuid() : cmd.HitId;
                }

                // Step 2: Register all molecules (SMILES may be null)
                var moleculeDTOs = batch.Select(cmd => new RegisterMoleculeDTO
                {
                    Name = cmd.MoleculeName,
                    SMILES = cmd.RequestedSMILES,
                    DisclosureStage = Daikon.Shared.Constants.Workflow.Stages.Screen,
                    DisclosureScientist = cmd.DisclosureScientist,
                    DisclosureOrgId = cmd.DisclosureOrgId

                }).ToList();

                var registrationResults = await _mLogixAPIService.RegisterBatch(moleculeDTOs);
                _logger.LogInformation("✅ Registered {Count} molecules", registrationResults.Count);

                //var resultMap = registrationResults.ToDictionary(r => r.Name, r => r);
                /* Index by Name and Synonyms as well */
                var resultMap = new Dictionary<string, MoleculeVM>(StringComparer.OrdinalIgnoreCase);
                foreach (var r in registrationResults)
                {
                    // Always add the official name
                    if (!string.IsNullOrWhiteSpace(r.Name))
                        resultMap[r.Name] = r;

                    // Add each synonym as a key too
                    if (!string.IsNullOrWhiteSpace(r.Synonyms))
                    {
                        var synonyms = r.Synonyms.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var syn in synonyms)
                        {
                            var key = syn.Trim();
                            if (!string.IsNullOrEmpty(key))
                                resultMap[key] = r;
                        }
                    }
                }



                // Step 3: Group all hits by HitCollectionId
                var grouped = batch.GroupBy(cmd => cmd.Id);

                foreach (var group in grouped)
                {
                    var hitCollectionId = group.Key;

                    try
                    {
                        var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(hitCollectionId);

                        foreach (var cmd in group)
                        {
                            var evt = _mapper.Map<HitAddedEvent>(cmd);
                            evt.RequestedMoleculeName = cmd.MoleculeName;
                            evt.IsStructureDisclosed = !string.IsNullOrWhiteSpace(cmd.RequestedSMILES?.Value);

                            if (resultMap.TryGetValue(cmd.MoleculeName, out var molReg))
                            {
                                evt.MoleculeId = molReg.Id;
                                evt.MoleculeRegistrationId = molReg.RegistrationId;
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ No registration result for molecule: {Name}", cmd.MoleculeName);
                            }

                            aggregate.AddHit(evt); // Raise multiple events before saving

                            /* Just for returning results */
                            Hit hitRes = _mapper.Map<Hit>(evt);
                            hitRes.HitCollectionId = hitCollectionId;
                            hitRes.Id = (Guid)cmd.HitId;
                            HitVM hitVM = _mapper.Map<HitVM>(hitRes, opts => opts.Items["WithMeta"] = false);
                            hitVM.Molecule = _mapper.Map<MoleculeVM>(resultMap[cmd.MoleculeName], opts => opts.Items["WithMeta"] = false);
                            addedHitsResults.Add(hitVM);
                        }

                        await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
                        _logger.LogInformation("✅ Saved {Count} hits for HitCollection {Id}", group.Count(), hitCollectionId);
                    }
                    catch (AggregateNotFoundException ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Aggregate not found for HitCollectionId: {Id}", hitCollectionId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Failed to register hits for HitCollectionId: {Id}", hitCollectionId);
                    }
                }
            }

            return addedHitsResults;
        }
    }
}
