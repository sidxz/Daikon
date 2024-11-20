
using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.DocuStore;
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

        public AddParsedDocHandler(
            IMapper mapper,
            ILogger<AddParsedDocHandler> logger,
            IParsedDocRepository parsedDocRepository,
            IEventSourcingHandler<ParsedDocAggregate> parsedDocEventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
            _parsedDocEventSourcingHandler = parsedDocEventSourcingHandler ?? throw new ArgumentNullException(nameof(parsedDocEventSourcingHandler));
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
