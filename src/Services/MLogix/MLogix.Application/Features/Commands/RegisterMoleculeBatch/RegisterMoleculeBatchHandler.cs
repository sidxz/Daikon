using AutoMapper;
using CQRS.Core.Extensions;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Commands.RegisterUndisclosed;
using MLogix.Domain.Aggregates;
using MLogix.Application.DTOs.CageFusion;
using MLogix.Application.Features.Commands.PredictNuisance;
using MLogix.Application.BackgroundServices;


namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    /* Small value object to hold disclosure info in a single place */
    public record DisclosureInfo(
        string Stage,
        string Scientist,
        Guid OrgId,
        string Reason,
        string? Notes,
        string LiteratureReferences,
        DateTime? StructureDisclosedDateUtc
    );

    /* Represents the original incoming ID, requested SMILES, and disclosure info */
    public record OriginalRequestInfo(
        Guid OriginalId,
        string? RequestedSmiles,
        DisclosureInfo Disclosure
    );

    public class RegisterMoleculeBatchHandler : IRequestHandler<RegisterMoleculeBatchCommand, List<RegisterMoleculeResponseDTO>>
    {
        private const int BatchSize = 500;

        private readonly IMapper _mapper;
        private readonly ILogger<RegisterMoleculeBatchHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _eventSourcingHandler;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        private readonly INuisanceJobQueue _nuisanceQueue;


        public RegisterMoleculeBatchHandler(
            IMapper mapper,
            ILogger<RegisterMoleculeBatchHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> eventSourcingHandler,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor,
            INuisanceJobQueue nuisanceQueue,
            IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _nuisanceQueue = nuisanceQueue ?? throw new ArgumentNullException(nameof(nuisanceQueue));
        }

        public async Task<List<RegisterMoleculeResponseDTO>> Handle(RegisterMoleculeBatchCommand request, CancellationToken cancellationToken)
        {

            if (request?.Commands == null || request.Commands.Count == 0)
                throw new ArgumentException("Molecule batch request must contain at least one command.");

            var requestHeaders = _httpContextAccessor.HttpContext?.Request?.Headers?
                .ToDictionary(h => h.Key, h => h.Value.ToString()) ?? new Dictionary<string, string>();

            var responses = new List<RegisterMoleculeResponseDTO>();
            var originalIdMapping = new Dictionary<Guid, OriginalRequestInfo>();

            /* STEP 1: Prepare and normalize commands */
            foreach (var cmd in request.Commands)
            {
                cmd.RegistrationId = cmd.RegistrationId == Guid.Empty ? Guid.NewGuid() : cmd.RegistrationId;
                cmd.Id = cmd.Id == Guid.Empty ? Guid.NewGuid() : cmd.Id;
                cmd.SetCreateProperties(request.RequestorUserId);

                var disclosedDate = cmd.StructureDisclosedDate == default
                    ? (DateTime?)null
                    : DateTime.SpecifyKind(cmd.StructureDisclosedDate, DateTimeKind.Utc);

                var disclosure = new DisclosureInfo(
                    Stage: (cmd.DisclosureStage ?? string.Empty).Trim(),
                    Scientist: (cmd.DisclosureScientist ?? string.Empty).Trim(),
                    OrgId: cmd.DisclosureOrgId,
                    Reason: (cmd.DisclosureReason ?? string.Empty).Trim(),
                    Notes: string.IsNullOrWhiteSpace(cmd.DisclosureNotes) ? null : cmd.DisclosureNotes.Trim(),
                    LiteratureReferences: (cmd.LiteratureReferences ?? string.Empty).Trim(),
                    StructureDisclosedDateUtc: disclosedDate
                );

                originalIdMapping[cmd.RegistrationId] = new OriginalRequestInfo(
                    OriginalId: cmd.Id,
                    RequestedSmiles: cmd.SMILES,
                    Disclosure: disclosure
                );

                // Use RegistrationId as the event-sourced Id for consistency downstream
                cmd.Id = cmd.RegistrationId;
            }

            LogOriginalMapping(originalIdMapping);

            /* STEP 2: Process in batches */
            foreach (var batch in request.Commands.Batch(BatchSize))
            {
                try
                {
                    foreach (var cmd in batch)
                        cmd.SetCreateProperties(request.RequestorUserId);

                    var undisclosedCommands = batch
                        .Where(c => string.IsNullOrEmpty(c.SMILES) && !string.IsNullOrEmpty(c.Name))
                        .Select(c => _mapper.Map<RegisterUndisclosedCommand>(c))
                        .ToList();

                    var disclosedCommands = batch
                        .Where(c => !string.IsNullOrEmpty(c.SMILES))
                        .ToList();

                    if (undisclosedCommands.Any())
                        await ProcessUndisclosedMoleculesAsync(undisclosedCommands, responses, cancellationToken);

                    if (disclosedCommands.Any())
                        await ProcessDisclosedMoleculesAsync(disclosedCommands, originalIdMapping, requestHeaders, responses, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Batch processing failed. Error: {Message}", ex.Message);
                    throw new InvalidOperationException("An error occurred while processing molecule batch.", ex);
                }
            }

            LogFinalResponses(responses);
            return responses;
        }

        /* Logs original mappings of RegistrationId to original Id, SMILES, and disclosure info */
        private void LogOriginalMapping(Dictionary<Guid, OriginalRequestInfo> originalMap)
        {
            _logger.LogInformation("==== ORIGINAL REQUEST ====");
            foreach (var entry in originalMap)
            {
                var d = entry.Value.Disclosure;
                _logger.LogInformation(
                    "RegistrationId: {RegId}, OriginalId: {Id}, SMILES: {SMILES}, Stage: {Stage}, Scientist: {Scientist}, OrgId: {OrgId}, Reason: {Reason}, DateUTC: {Date}, Refs: {Refs}",
                    entry.Key,
                    entry.Value.OriginalId,
                    entry.Value.RequestedSmiles,
                    d.Stage,
                    d.Scientist,
                    d.OrgId,
                    d.Reason,
                    d.StructureDisclosedDateUtc,
                    d.LiteratureReferences
                );
            }
            _logger.LogInformation("==========================");
        }

        /* Handles undisclosed molecule registration one-by-one via mediator */
        private async Task ProcessUndisclosedMoleculesAsync(
            List<RegisterUndisclosedCommand> undisclosedBatch,
            List<RegisterMoleculeResponseDTO> allResponses,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing undisclosed molecules: Count = {Count}", undisclosedBatch.Count);

            foreach (var cmd in undisclosedBatch)
            {
                var result = await _mediator.Send(cmd, cancellationToken);
                var responseDto = _mapper.Map<RegisterMoleculeResponseDTO>(result);
                allResponses.Add(responseDto);
            }
        }

        /* Handles disclosed molecule registration via API and event sourcing */
        private async Task ProcessDisclosedMoleculesAsync(
            List<RegisterMoleculeCommandWithRegId> disclosedBatch,
            Dictionary<Guid, OriginalRequestInfo> originalMap,
            Dictionary<string, string> headers,
            List<RegisterMoleculeResponseDTO> allResponses,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing disclosed molecules: Count = {Count}", disclosedBatch.Count);
            var currentTime = DateTime.UtcNow;

            var apiResponses = await _moleculeAPI.RegisterBatch(disclosedBatch, headers);

            _logger.LogInformation("Molecule API responded with {Count} entries", apiResponses.Count);

            List<NuisanceRequestTuple> checkForNuisanceList = [];

            // Requestor user id from header (for audit)
            Guid requestorUserId = Guid.Empty;
            if (headers.TryGetValue("AppUser-Id", out var requestorUserId_FromHeader) &&
                Guid.TryParse(requestorUserId_FromHeader, out var parsedRequestor))
            {
                requestorUserId = parsedRequestor;
            }

            foreach (var apiResponse in apiResponses)
            {

                var existing = await _moleculeRepository.GetMoleculeByRegistrationId(apiResponse.Id);
                RegisterMoleculeResponseDTO dto;

                if (existing != null)
                {
                    dto = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                    dto.WasAlreadyRegistered = true;
                    dto.Id = existing.Id;
                    dto.RegistrationId = apiResponse.Id;

                    _logger.LogInformation("Molecule '{Name}' already registered.", apiResponse.Name);
                }
                else
                {
                    var newEvent = _mapper.Map<MoleculeCreatedEvent>(apiResponse);

                    if (!originalMap.TryGetValue(apiResponse.Id, out var original))
                    {
                        _logger.LogWarning("Missing original mapping for Name: {Name}, RegistrationId: {Id}", apiResponse.Name, apiResponse.Id);
                        throw new InvalidOperationException($"Missing original mapping for  Name: {apiResponse.Name}, RegistrationId: {apiResponse.Id}");
                    }

                    // carry through original IDs/inputs

                    newEvent.RequestorUserId = requestorUserId;
                    newEvent.CreatedById = requestorUserId;
                    newEvent.DateCreated = currentTime;
                    newEvent.IsModified = false;
                    newEvent.LastModifiedById = requestorUserId;
                    newEvent.DateModified = currentTime;

                    newEvent.Id = original.OriginalId;
                    newEvent.RequestedSMILES = original.RequestedSmiles;
                    newEvent.RegistrationId = apiResponse.Id;
                    newEvent.SmilesCanonical = apiResponse.SmilesCanonical;

                    var aggregate = new MoleculeAggregate(newEvent);

                    // build MoleculeDisclosedEvent with resolved disclosure fields
                    var disclosure = original.Disclosure;

                    // Try to get "AppUser-FullName" from headers; used as fallback for scientist
                    var resolvedScientist = ResolveScientist(disclosure, headers);

                    // OrgId precedence: command > header > Guid.Empty
                    var resolvedOrgId = ResolveOrgId(disclosure, headers);



                    // Disclosure date precedence: command > event created > now UTC
                    var resolvedDate = ResolveDisclosedDate(disclosure, newEvent.DateCreated);

                    var moleculeDisclosedEvent = new MoleculeDisclosedEvent
                    {
                        RequestorUserId = requestorUserId,
                        CreatedById = requestorUserId,
                        DateCreated = currentTime,
                        IsModified = false,
                        LastModifiedById = requestorUserId,
                        DateModified = currentTime,


                        Id = newEvent.Id,
                        RegistrationId = newEvent.RegistrationId,
                        Name = newEvent.Name,
                        RequestedSMILES = newEvent.RequestedSMILES,
                        SmilesCanonical = newEvent.SmilesCanonical,

                        IsStructureDisclosed = true,
                        StructureDisclosedDate = resolvedDate,
                        StructureDisclosedByUserId = requestorUserId,

                        // Disclosure bundle (now includes additional fields)
                        DisclosureScientist = resolvedScientist,
                        DisclosureOrgId = resolvedOrgId,
                        DisclosureStage = disclosure.Stage,
                        DisclosureReason = string.IsNullOrWhiteSpace(disclosure.Reason) ? "Automatic registration" : disclosure.Reason,
                        DisclosureNotes = disclosure.Notes,
                        LiteratureReferences = disclosure.LiteratureReferences
                    };

                    aggregate.DiscloseMolecule(moleculeDisclosedEvent);
                    await _eventSourcingHandler.SaveAsync(aggregate);

                    dto = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                    dto.WasAlreadyRegistered = false;
                    dto.Id = newEvent.Id;
                    dto.RegistrationId = apiResponse.Id;

                    checkForNuisanceList.Add(new NuisanceRequestTuple
                    {
                        Id = newEvent.Id.ToString(),
                        SMILES = dto.SmilesCanonical
                    });
                }

                allResponses.Add(dto);
            }
            // Trigger nuisance prediction for newly registered molecules
            if (checkForNuisanceList.Count > 0)
            {

                var nuisanceCommand = new PredictNuisanceCommand
                {
                    NuisanceRequestTuple = checkForNuisanceList,
                    PlotAllAttention = false,
                    RequestorUserId = requestorUserId,
                    CreatedById = requestorUserId,
                    DateCreated = currentTime,
                    IsModified = false,
                    LastModifiedById = requestorUserId,
                    DateModified = currentTime,
                };
                try
                {
                    var correlationId = Guid.NewGuid().ToString("N");
                    using (_logger.BeginScope(new Dictionary<string, object> { ["corrId"] = correlationId }))
                    {
                        var job = new NuisanceJob(nuisanceCommand, correlationId);
                        await _nuisanceQueue.EnqueueAsync(job, CancellationToken.None);
                        _logger.LogInformation("Queued nuisance prediction for {Count} molecules.", checkForNuisanceList.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to trigger nuisance predictions.");
                }

            }

        }

        /* Helper resolvers keep precedence rules explicit and testable */
        private static string ResolveScientist(DisclosureInfo fromCmd, IDictionary<string, string> headers)
        {
            if (!string.IsNullOrWhiteSpace(fromCmd.Scientist)) return fromCmd.Scientist;
            return headers.TryGetValue("AppUser-FullName", out var fullName) ? fullName : string.Empty;
        }

        private static Guid ResolveOrgId(DisclosureInfo fromCmd, IDictionary<string, string> headers)
        {
            if (fromCmd.OrgId != Guid.Empty) return fromCmd.OrgId;
            return headers.TryGetValue("AppOrg-Id", out var v) && Guid.TryParse(v, out var gid) ? gid : Guid.Empty;
        }

        private static DateTime ResolveDisclosedDate(DisclosureInfo fromCmd, DateTime? eventCreatedUtc)
        {
            // preference: explicit command date (already normalized to UTC), else event created date, else now
            return fromCmd.StructureDisclosedDateUtc
                ?? eventCreatedUtc
                ?? DateTime.UtcNow;
        }

        /* Logs all molecule registration responses */
        private void LogFinalResponses(IEnumerable<RegisterMoleculeResponseDTO> responses)
        {
            foreach (var res in responses)
            {
                _logger.LogInformation("Registered Molecule: {Name}, Id: {Id}, RegId: {RegId}, AlreadyRegistered: {Status}",
                    res.Name, res.Id, res.RegistrationId, res.WasAlreadyRegistered);
            }
        }
    }
}
