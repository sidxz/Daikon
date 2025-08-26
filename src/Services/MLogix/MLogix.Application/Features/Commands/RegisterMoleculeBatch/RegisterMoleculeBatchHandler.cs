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

namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    /* Represents the original incoming ID and SMILES of a molecule */
    public record OriginalRequestInfo(Guid OriginalId, string? SMILES, string? DisclosureStage, string? DisclosureScientist, Guid DisclosureOrgId);

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

        public RegisterMoleculeBatchHandler(
            IMapper mapper,
            ILogger<RegisterMoleculeBatchHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> eventSourcingHandler,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor,
            IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

                originalIdMapping[cmd.RegistrationId] = new OriginalRequestInfo(cmd.Id, cmd.SMILES, cmd.DisclosureStage, cmd.DisclosureScientist, cmd.DisclosureOrgId);
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

        /* Logs original mappings of RegistrationId to original Id and SMILES */
        private void LogOriginalMapping(Dictionary<Guid, OriginalRequestInfo> originalMap)
        {
            _logger.LogInformation("==== ORIGINAL REQUEST ====");
            foreach (var entry in originalMap)
            {
                _logger.LogInformation("RegistrationId: {RegId}, OriginalId: {Id}, SMILES: {SMILES}, DisclosureScientist: {DisclosureScientist}, DisclosureOrgId: {DisclosureOrgId}",
                    entry.Key, entry.Value.OriginalId, entry.Value.SMILES, entry.Value.DisclosureScientist, entry.Value.DisclosureOrgId);
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

            var apiResponses = await _moleculeAPI.RegisterBatch(disclosedBatch, headers);

            _logger.LogInformation("Molecule API responded with {Count} entries", apiResponses.Count);

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
                    string disclosedStage = string.Empty;
                    string disclosureScientist = string.Empty;
                    Guid disclosureOrgId = Guid.Empty;


                    if (originalMap.TryGetValue(apiResponse.Id, out var original))
                    {
                        newEvent.Id = original.OriginalId;
                        newEvent.RequestedSMILES = original.SMILES;
                        disclosedStage = original.DisclosureStage ?? string.Empty;
                        disclosureScientist = original.DisclosureScientist ?? string.Empty;
                        disclosureOrgId = original.DisclosureOrgId;
                    }
                    else
                    {
                        _logger.LogWarning("Missing original mapping for RegistrationId: {Id}", apiResponse.Id);
                        throw new InvalidOperationException($"Missing original mapping for RegistrationId: {apiResponse.Id}");
                    }

                    newEvent.RegistrationId = apiResponse.Id;
                    newEvent.SmilesCanonical = apiResponse.SmilesCanonical;

                    var aggregate = new MoleculeAggregate(newEvent);

                    // try to get "AppUser-FullName" from headers
                    string scientistFromHeader = string.Empty;
                    if (headers.TryGetValue("AppUser-FullName", out var fullName))
                    {
                        scientistFromHeader = fullName;
                    }

                    string resolvedScientist = string.IsNullOrEmpty(disclosureScientist) ? scientistFromHeader : disclosureScientist;

                    Guid disclosureOrgIdFromHeader = Guid.Empty;
                    if (headers.TryGetValue("AppOrg-Id", out var disclosureOrgId_FromHeader))
                    {
                        disclosureOrgIdFromHeader = Guid.Parse(disclosureOrgId_FromHeader);
                    }

                    Guid resolvedOrgId = disclosureOrgId == Guid.Empty ? disclosureOrgIdFromHeader : disclosureOrgId;

                    Guid requestorUserId = Guid.Empty;
                    if (headers.TryGetValue("AppUser-Id", out var requestorUserId_FromHeader))
                    {
                        requestorUserId = Guid.Parse(requestorUserId_FromHeader);
                    }

                    var moleculeDisclosedEvent = new MoleculeDisclosedEvent
                    {
                        RequestorUserId = newEvent.RequestorUserId,
                        Id = newEvent.Id,
                        RegistrationId = newEvent.RegistrationId,
                        Name = newEvent.Name,
                        RequestedSMILES = newEvent.RequestedSMILES,
                        SmilesCanonical = newEvent.SmilesCanonical,
                        IsStructureDisclosed = true,
                        StructureDisclosedDate = newEvent.DateCreated ?? DateTime.UtcNow,
                        DisclosureScientist = resolvedScientist,
                        DisclosureOrgId = resolvedOrgId,
                        DisclosureReason = "Automatic registration",
                        DisclosureStage = disclosedStage,
                        StructureDisclosedByUserId = requestorUserId,
                    };

                    aggregate.DiscloseMolecule(moleculeDisclosedEvent);
                    await _eventSourcingHandler.SaveAsync(aggregate);

                    dto = _mapper.Map<RegisterMoleculeResponseDTO>(apiResponse);
                    dto.WasAlreadyRegistered = false;
                    dto.Id = newEvent.Id;
                    dto.RegistrationId = apiResponse.Id;

                }

                allResponses.Add(dto);
            }
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
