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
using MLogix.Application.Features.Commands.DiscloseMolecule;


namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{

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

            // lets create a deep copy of commands for future reference
            
            

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

                    var commandsCopy = new List<RegisterMoleculeCommandWithRegId>(batch);

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
                            // Return removed compounds as already registered in response
                            responses.AddRange(existingInChemVault.Select(m => _mapper.Map<RegisterMoleculeResponseDTO>(m)));
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
                            _logger.LogInformation("Existing compound names in MLogix: {Names}", string.Join(", ", existingNamesInMLogix));
                        }

                        // now whatever is left in requestedCompoundsWithoutSmiles are truly new compounds without SMILES
                        // Register them as undisclosed
                        var undisclosedCommands = requestedCompoundsWithoutSmiles
                            .Select(c => _mapper.Map<RegisterUndisclosedCommand>(c))
                            .ToList();
                        if (undisclosedCommands.Count > 0)
                        {
                            await ProcessUndisclosedMoleculesAsync(undisclosedCommands, responses, cancellationToken);
                        }
                        _logger.LogInformation("Registering {Count} new compounds without SMILES as undisclosed.", requestedCompoundsWithoutSmiles.Count);
                        _logger.LogInformation("New compound names to register as undisclosed: {Names}", string.Join(", ", requestedCompoundsWithoutSmiles.Select(c => c.Name)));
                    }


                    /* Deal with Requested Compounds WITH SMILES */
                    if (requestedCompoundsWithSmiles.Count > 0)
                    {
                        List<NuisanceRequestTuple> checkForNuisanceList = [];

                        // Lets check which of these exist in ChemVault by SMILES
                        var smilesToCheck = requestedCompoundsWithSmiles
                            .Select(c => c.SMILES)
                            .Where(smiles => !string.IsNullOrEmpty(smiles))
                            .Distinct()
                            .Cast<string>()
                            .ToList();
                        var existingInChemVaultBySmiles = await _moleculeAPI.GetMoleculesBySMILES(smilesToCheck, requestHeaders);

                        // Now if we find direct name or synonym matches in ChemVault, we will consider them existing and remove them from registration list
                        List<string> existingMoleculeNamesInChemVault = existingInChemVaultBySmiles
                            .Select(m => m.Name)
                            .Concat(existingInChemVaultBySmiles.SelectMany(m => ExtractSynonyms(m.Synonyms)))
                            .Distinct()
                            .ToList();

                        if (existingMoleculeNamesInChemVault.Count > 0)
                        {
                            // These are exact matches, lets remove them from requestedCompoundsWithSmiles
                            requestedCompoundsWithSmiles.RemoveAll(c => existingMoleculeNamesInChemVault.Contains(c.Name));
                            _logger.LogInformation("Found {Count} compounds with SMILES already existing in ChemVault by SMILES.", existingMoleculeNamesInChemVault.Count);
                            _logger.LogInformation("Existing compound names in ChemVault: {Names}", string.Join(", ", existingMoleculeNamesInChemVault));
                        }

                        // Here these are new compounds or new synonyms
                        // possibilities are:
                        // 1. Completely New Compound To DAIKON
                        // 2. Existing Compound in DAIKON but New Synonym
                        // 3. Name known (Undisclosed), so now disclosing structure

                        // Lets find  undisclosed ones in MLogix DB by Name
                        var namesToCheckInMlogix = requestedCompoundsWithSmiles
                            .Select(c => c.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .Distinct()
                            .Cast<string>()
                            .ToList();
                        var existingInMLogixByName = await _moleculeRepository.GetByNames(namesToCheckInMlogix);
                        var existingUndisclosedNamesInMLogixByName = existingInMLogixByName
                            .Where(m => string.IsNullOrEmpty(m.RequestedSMILES))
                            .Select(m => m.Name)
                            .Distinct()
                            .ToList();
                        // Lets now copy them to a new list and remove from requestedCompoundsWithSmiles
                        var toBeDisclosedNow = requestedCompoundsWithSmiles
                            .Where(c => existingUndisclosedNamesInMLogixByName.Contains(c.Name))
                            .ToList();
                        if (toBeDisclosedNow.Count > 0)
                        {
                            requestedCompoundsWithSmiles.RemoveAll(c => existingUndisclosedNamesInMLogixByName.Contains(c.Name));
                            _logger.LogInformation("Found {Count} compounds with SMILES already existing in MLogix by name for disclosure.", toBeDisclosedNow.Count);

                            //Use Disclosure Command to disclose these structures (SMILES might exists with different name, let disclosure handle that)

                            var discloseBatchCmd = new DiscloseMoleculeBatchCommand
                            {
                                Molecules = toBeDisclosedNow.Select(c => _mapper.Map<DiscloseMoleculeCommand>(c)).ToList()
                            };

                            var discloseResult = await _mediator.Send(discloseBatchCmd, cancellationToken);
                            responses.AddRange(discloseResult.Select(d => _mapper.Map<RegisterMoleculeResponseDTO>(d)));
                            checkForNuisanceList.AddRange(discloseResult.Select(d => new NuisanceRequestTuple
                            {
                                Id = d.Id.ToString(),
                                SMILES = d.SmilesCanonical
                            }));
                        }

                        // Rest are either new compounds or new synonyms
                        // Lets register them in ChemVault

                        // If existing in ChemVault we do nothing, synonym would be automatically added
                        // For new ones we create new registration and disclosure event

                        var currentTime = DateTime.UtcNow;

                        List<RegisterMoleculeCommandWithRegId> requestCompoundWithSmilesIDmapped = [];
                        foreach (var cmd in requestedCompoundsWithSmiles)
                        {
                            // map to new object to avoid modifying original command
                            var newCmd = _mapper.Map<RegisterMoleculeCommandWithRegId>(cmd);
                            newCmd.Id = cmd.RegistrationId;
                            requestCompoundWithSmilesIDmapped.Add(newCmd);
                        }

                        var registerMoleculeResponses = await _moleculeAPI.RegisterBatch(requestCompoundWithSmilesIDmapped, requestHeaders);

                        _logger.LogInformation("ChemVault API responded with {Count} entries", registerMoleculeResponses.Count);

                    
                        foreach (var apiResponse in registerMoleculeResponses)
                        {

                            var existingInMLogix = await _moleculeRepository.GetMoleculeByRegistrationId(apiResponse.Id);
                            RegisterMoleculeResponseDTO dto;

                            if (existingInMLogix != null)
                            {
                                dto = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                                dto.WasAlreadyRegistered = true;
                                dto.Id = existingInMLogix.Id;
                                dto.RegistrationId = apiResponse.Id;

                                _logger.LogInformation("Molecule '{Name}' already registered. New Synonyms if any added", apiResponse.Name);
                                responses.Add(dto);
                            }
                            else
                            {
                                var originalCmd = commandsCopy
                                    .FirstOrDefault(c => c.RegistrationId == apiResponse.Id);
                                if (originalCmd == null)
                                {
                                    _logger.LogWarning("Missing original command for Name: {Name}, RegistrationId: {Id}", apiResponse.Name, apiResponse.Id);
                                    throw new InvalidOperationException($"Missing original command for Name: {apiResponse.Name}, RegistrationId: {apiResponse.Id}");
                                }


                                var moleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(apiResponse);
                                // carry through original IDs/inputs

                                moleculeCreatedEvent.RequestorUserId = request.RequestorUserId;
                                moleculeCreatedEvent.CreatedById = request.RequestorUserId;
                                moleculeCreatedEvent.DateCreated = currentTime;
                                moleculeCreatedEvent.IsModified = false;
                                moleculeCreatedEvent.LastModifiedById = request.RequestorUserId;
                                moleculeCreatedEvent.DateModified = currentTime;

                                moleculeCreatedEvent.Id = originalCmd.Id;
                                moleculeCreatedEvent.RequestedSMILES = originalCmd.SMILES;
                                moleculeCreatedEvent.RegistrationId = apiResponse.Id;
                                moleculeCreatedEvent.SmilesCanonical = apiResponse.SmilesCanonical;

                                var aggregate = new MoleculeAggregate(moleculeCreatedEvent);

                                // Now lets create Disclosure Event as these all have SMILES

                                string scientistFromHeaders = string.Empty;
                                if (requestHeaders.TryGetValue("AppUser-FullName", out var fullName))
                                {
                                    scientistFromHeaders = fullName;
                                }
                                Guid disclosureOrgIdFromHeaders = Guid.Empty;
                                if (requestHeaders.TryGetValue("AppOrg-Id", out var disclosureOrgIdFromHeader))
                                {
                                    disclosureOrgIdFromHeaders = Guid.Parse(disclosureOrgIdFromHeader);
                                }
                                var moleculeDisclosedEvent = new MoleculeDisclosedEvent
                                {
                                    RequestorUserId = request.RequestorUserId,
                                    CreatedById = request.RequestorUserId,
                                    DateCreated = currentTime,
                                    IsModified = false,
                                    LastModifiedById = request.RequestorUserId,
                                    DateModified = currentTime,
                                    RegistrationId = moleculeCreatedEvent.RegistrationId,
                                    Name = moleculeCreatedEvent.Name,
                                    RequestedSMILES = moleculeCreatedEvent.RequestedSMILES,
                                    SmilesCanonical = moleculeCreatedEvent.SmilesCanonical,
                                    IsStructureDisclosed = true,
                                    StructureDisclosedDate = originalCmd.StructureDisclosedDate != default ? originalCmd.StructureDisclosedDate : currentTime,
                                    StructureDisclosedByUserId = request.RequestorUserId,
                                    // Disclosure bundle
                                    DisclosureScientist = originalCmd.DisclosureScientist ?? scientistFromHeaders,
                                    DisclosureOrgId = originalCmd.DisclosureOrgId != Guid.Empty ? originalCmd.DisclosureOrgId : disclosureOrgIdFromHeaders,
                                    DisclosureStage = originalCmd.DisclosureStage,
                                    DisclosureReason = string.IsNullOrWhiteSpace(originalCmd.DisclosureReason) ? "Automatic registration" : originalCmd.DisclosureReason,
                                    DisclosureNotes = originalCmd.DisclosureNotes,
                                    LiteratureReferences = originalCmd.LiteratureReferences
                                };

                                aggregate.DiscloseMolecule(moleculeDisclosedEvent);
                                await _eventSourcingHandler.SaveAsync(aggregate);

                                dto = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                                dto.WasAlreadyRegistered = false;
                                dto.Id = moleculeCreatedEvent.Id;
                                dto.RegistrationId = apiResponse.Id;

                                checkForNuisanceList.Add(new NuisanceRequestTuple
                                {
                                    Id = moleculeCreatedEvent.Id.ToString(),
                                    SMILES = dto.SmilesCanonical
                                });

                                responses.Add(dto);
                            }

                        }

                        if (checkForNuisanceList.Count > 0)
                        {
                            _logger.LogInformation("Preparing to trigger nuisance predictions for {Count} molecules.", checkForNuisanceList.Count);

                            var nuisanceCommand = new PredictNuisanceCommand
                            {
                                NuisanceRequestTuple = checkForNuisanceList,
                                PlotAllAttention = false,
                                RequestorUserId = request.RequestorUserId,
                                CreatedById = request.RequestorUserId,
                                DateCreated = currentTime,
                                IsModified = false,
                                LastModifiedById = request.RequestorUserId,
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
                cmd.ChemVaultCheck = false; // skip ChemVault check as already done
                var result = await _mediator.Send(cmd, cancellationToken);
                var responseDto = _mapper.Map<RegisterMoleculeResponseDTO>(result);
                allResponses.Add(responseDto);
            }
        }
    }
}