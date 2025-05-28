
using AutoMapper;
using Daikon.EventStore.Handlers;
using Daikon.Events.DocuStore;
using Daikon.Shared.APIClients.MLogix;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Aggregates;
using DocuStore.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DocuStore.Application.Features.Commands.AddParsedDoc
{
    public class AddParsedDocHandler : IRequestHandler<AddParsedDocCommand, ParsedDoc>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddParsedDocHandler> _logger;
        private readonly IParsedDocRepository _parsedDocRepository;
        private readonly IEventSourcingHandler<ParsedDocAggregate> _parsedDocEventSourcingHandler;
        private readonly IMLogixAPI _mLogixAPIService;

        public AddParsedDocHandler(
            IMapper mapper,
            ILogger<AddParsedDocHandler> logger,
            IParsedDocRepository parsedDocRepository,
            IEventSourcingHandler<ParsedDocAggregate> parsedDocEventSourcingHandler,
            IMLogixAPI mLogixAPIService
            )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
            _parsedDocEventSourcingHandler = parsedDocEventSourcingHandler ?? throw new ArgumentNullException(nameof(parsedDocEventSourcingHandler));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
        }

        public async Task<ParsedDoc> Handle(AddParsedDocCommand request, CancellationToken cancellationToken)
        {
            // Validate input

            if (string.IsNullOrWhiteSpace(request.FilePath))
            {
                _logger.LogWarning("FilePath is missing or empty.");
                throw new InvalidOperationException("FilePath is required.");
            }

            try
            {
                // Check if the document already exists in the repository
                var existingDoc = await _parsedDocRepository.ReadByPath(request.FilePath);
                if (existingDoc != null)
                {
                    _logger.LogWarning("Document already exists: {FilePath}", request.FilePath);
                    throw new InvalidOperationException($"A document with the path '{request.FilePath}' already exists.");
                }

                // Set metadata for the new document
                request.SetCreateProperties(request.RequestorUserId);

                // map molecule names to IDs
                foreach (var smiles in request.ExtractedSMILES)
                {
                    var molecule = await _mLogixAPIService.GetMoleculeBySmiles(smiles);
                    if (molecule != null)
                    {
                        string moleculeId = molecule.Id.ToString();
                        string moleculeName = molecule.Name;
                        request.Molecules.TryAdd(moleculeId, moleculeName);
                        request.Tags.Add(moleculeName);
                    }
                }

                // Map request to an event and initialize the aggregate
                var parsedDocAddedEvent = _mapper.Map<ParsedDocAddedEvent>(request);
                var aggregate = new ParsedDocAggregate(parsedDocAddedEvent);

                // Persist the aggregate through the event sourcing handler
                await _parsedDocEventSourcingHandler.SaveAsync(aggregate);

                // Map the event back to a ParsedDoc entity for the response
                var parsedDoc = _mapper.Map<ParsedDoc>(parsedDocAddedEvent);

                _logger.LogInformation("Successfully added parsed document: {FilePath}", request.FilePath);
                return parsedDoc;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for AddParsedDocCommand.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing AddParsedDocCommand.");
                throw new ApplicationException("An error occurred while adding the parsed document. Please try again later.", ex);
            }
        }
    }
}
