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
using MLogix.Application.Features.Queries.GetMolecules.ByIDs;
using Daikon.Shared.VM.MLogix;
using MLogix.Application.Features.Queries.GetMolecules.BySMILES;


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

            /* STEP 1: Prepare and normalize commands */
            foreach (var cmd in request.Commands)
            {
                cmd.RegistrationId = cmd.RegistrationId == Guid.Empty ? Guid.NewGuid() : cmd.RegistrationId;
                cmd.Id = cmd.Id == Guid.Empty ? Guid.NewGuid() : cmd.Id;
                cmd.SetCreateProperties(request.RequestorUserId);

            }

            var responses = new List<RegisterMoleculeResponseDTO>();


            /* STEP 2: Process in batches */
            foreach (var batch in request.Commands.Batch(BatchSize))
            {
                try
                {
                    foreach (var cmd in batch)
                        cmd.SetCreateProperties(request.RequestorUserId);



                    var requestedCompoundsWithoutSmiles = batch
                        .Where(c => string.IsNullOrEmpty(c.SMILES) && !string.IsNullOrEmpty(c.Name))
                        .ToList();

                    var requestedCompoundsWithSmiles = batch
                        .Where(c => !string.IsNullOrEmpty(c.SMILES))
                        .ToList();

                    var toRegisterAsUndisclosed = new List<RegisterUndisclosedCommand>();

                    /* Deal with Requested Compounds WITHOUT SMILES */

                    if (requestedCompoundsWithoutSmiles.Count > 0)
                    {
                        // lets check by name with ChemVault if they exist, ChemVault looks for both names and synonyms internally
                        var namesToCheck = requestedCompoundsWithoutSmiles
                            .Select(c => c.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .Distinct()
                            .Cast<string>()
                            .ToList();
                        var existingInChemVault = await _moleculeAPI.FindByNamesOrSynonymsExact(namesToCheck, requestHeaders);

                        // Now from requestedCompoundsWithoutSmiles, lets remove those that exist in ChemVault
                        // Note that to compare namesToCheck with existingInChemVault, we need to check both Name and Synonyms

                        List<string> existingInChemVaultNames = [];
                        foreach (var mol in existingInChemVault)
                        {
                            existingInChemVaultNames.Add(mol.Name);
                            var synonymsList = ExtractSynonyms(mol.Synonyms);
                            existingInChemVaultNames.AddRange(synonymsList);
                        }
                        // now remove them from requestedCompoundsWithoutSmiles list
                        if (existingInChemVaultNames.Count > 0)
                        {
                            requestedCompoundsWithoutSmiles.RemoveAll(c => existingInChemVaultNames.Contains(c.Name));
                            _logger.LogInformation("Found {Count} compounds without SMILES already existing in ChemVault by name/synonym.", namesToCheck.Count - requestedCompoundsWithoutSmiles.Count);
                            _logger.LogInformation("Existing compound names/synonyms in ChemVault: {Names}", string.Join(", ", existingInChemVaultNames));
                            //TODO: Return removed compounds as already registered in response
                        }

                        // now requestedCompoundsWithoutSmiles contains only those that do not exist in ChemVault
                        // Now lets find out which of these exist in our local MLogix DB
                        var namesToCheckInMLogix = requestedCompoundsWithoutSmiles
                            .Select(c => c.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .Distinct()
                            .Cast<string>()
                            .ToList();
                        var existingInMLogix = await _moleculeRepository.GetByNames(namesToCheckInMLogix);

                        // now remove those that exist in MLogix from requestedCompoundsWithoutSmiles
                        if (existingInMLogix.Count > 0)
                        {
                            var existingNamesInMLogix = existingInMLogix.Select(m => m.Name).Distinct().ToList();
                            requestedCompoundsWithoutSmiles.RemoveAll(c => existingNamesInMLogix.Contains(c.Name));
                            _logger.LogInformation("Found {Count} compounds without SMILES already existing in MLogix by name.", existingNamesInMLogix.Count);
                            // dump the names to log
                            _logger.LogInformation("Existing compound names in MLogix: {Names}", string.Join(", ", existingNamesInMLogix));

                        }

                        // now whatever is left in requestedCompoundsWithoutSmiles are truly new compounds without SMILES
                        // ToDo: we will register them as undisclosed
                        _logger.LogInformation("Registering {Count} new compounds without SMILES as undisclosed.", requestedCompoundsWithoutSmiles.Count);
                        _logger.LogInformation("New compound names to register as undisclosed: {Names}", string.Join(", ", requestedCompoundsWithoutSmiles.Select(c => c.Name)));
                    }


                    /* Deal with Requested Compounds WITH SMILES */
                    if (requestedCompoundsWithSmiles.Count > 0)
                    {
                        // 
                        
                    }






                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Batch processing failed. Error: {Message}", ex.Message);
                    throw new InvalidOperationException("An error occurred while processing molecule batch.", ex);
                }
            }

            return responses;
        }

        private List<string> ExtractSynonyms(string synonyms)
        {
            // Remove white space from split strings as well
            var synonymList = synonyms.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            return synonymList;
        }

        /* Logs original mappings of RegistrationId to original Id, SMILES, and disclosure info */


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






    }
}
