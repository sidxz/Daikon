
using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.DocuStore;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Aggregates;
using DocuStore.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DocuStore.Application.Features.Commands.UpdateParsedDoc
{
    public class UpdateParsedDocHandler : IRequestHandler<UpdateParsedDocCommand, ParsedDoc>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateParsedDocHandler> _logger;
        private readonly IParsedDocRepository _parsedDocRepository;
        private readonly IEventSourcingHandler<ParsedDocAggregate> _parsedDocEventSourcingHandler;

        public UpdateParsedDocHandler(
            IMapper mapper,
            ILogger<UpdateParsedDocHandler> logger,
            IParsedDocRepository parsedDocRepository,
            IEventSourcingHandler<ParsedDocAggregate> parsedDocEventSourcingHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
            _parsedDocEventSourcingHandler = parsedDocEventSourcingHandler ?? throw new ArgumentNullException(nameof(parsedDocEventSourcingHandler));
        }

        public async Task<ParsedDoc> Handle(UpdateParsedDocCommand request, CancellationToken cancellationToken)
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
                if (existingDoc == null)
                {
                    _logger.LogWarning("Document does not exists: {FilePath}", request.FilePath);
                    throw new InvalidOperationException($"A document with the path '{request.FilePath}' does not exists.");
                }

                // Set metadata for the new document
                request.SetUpdateProperties(request.RequestorUserId);

                // Map request to an event and initialize the aggregate
                var parsedDocUpdatedEvent = _mapper.Map<ParsedDocUpdatedEvent>(request);
                var aggregate = await _parsedDocEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateParsedDoc(parsedDocUpdatedEvent);

                // Persist the aggregate through the event sourcing handler
                await _parsedDocEventSourcingHandler.SaveAsync(aggregate);

                // Map the event back to a ParsedDoc entity for the response
                var parsedDoc = _mapper.Map<ParsedDoc>(parsedDocUpdatedEvent);

                _logger.LogInformation("Successfully updated parsed document: {FilePath}", request.FilePath);
                return parsedDoc;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for UpdateParsedDocCommand.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing UpdateParsedDocCommand.");
                throw new ApplicationException("An error occurred while adding the parsed document. Please try again later.", ex);
            }
        }
    }
}
