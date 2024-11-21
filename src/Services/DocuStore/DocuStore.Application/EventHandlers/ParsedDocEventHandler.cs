
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.DocuStore;
using DocuStore.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace DocuStore.Application.EventHandlers
{
    public class ParsedDocEventHandler : IParsedDocEventHandler
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ParsedDocEventHandler> _logger;
        private readonly IParsedDocRepository _parsedDocRepository;

        public ParsedDocEventHandler(IMapper mapper, ILogger<ParsedDocEventHandler> logger, IParsedDocRepository parsedDocRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _parsedDocRepository = parsedDocRepository;
        }

        public async Task OnEvent(ParsedDocAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: ParsedDocAddedEvent: {Id}", @event.Id);
            var parsedDoc = _mapper.Map<Domain.Entities.ParsedDoc>(@event);
            parsedDoc.Id = @event.Id;

            try
            {
                await _parsedDocRepository.Create(parsedDoc);
            }
            catch (Exception ex)
            {
                throw new EventHandlerException(nameof(ParsedDocEventHandler), "ParsedDocAddedEvent Error creating parsed document", ex);
            }
        }

        public async Task OnEvent(ParsedDocUpdatedEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event), "ParsedDocUpdatedEvent cannot be null.");
            }

            _logger.LogInformation("Handling ParsedDocUpdatedEvent: {Id}", @event.Id);

            try
            {
                // Try to find the existing document
                var existingParsedDoc = await _parsedDocRepository.ReadById(@event.Id);

                if (existingParsedDoc == null)
                {
                    _logger.LogWarning("ParsedDoc with ID {Id} not found. Creating a new document.", @event.Id);

                    // Map the event directly to a new ParsedDoc instance
                    var newParsedDoc = _mapper.Map<Domain.Entities.ParsedDoc>(@event);
                    newParsedDoc.Id = @event.Id;

                    // Set creation metadata
                    newParsedDoc.DateCreated = DateTime.UtcNow;
                    newParsedDoc.CreatedById = @event.RequestorUserId;

                    await _parsedDocRepository.Create(newParsedDoc);
                    _logger.LogInformation("Successfully created ParsedDoc with ID: {Id}", @event.Id);
                }
                else
                {
                    _logger.LogInformation("ParsedDoc with ID {Id} found. Updating the document.", @event.Id);

                    // Update the existing document
                    var updatedParsedDoc = _mapper.Map(@event, existingParsedDoc);

                    // Preserve original creation metadata
                    updatedParsedDoc.DateCreated = existingParsedDoc.DateCreated;
                    updatedParsedDoc.CreatedById = existingParsedDoc.CreatedById;

                    await _parsedDocRepository.Update(updatedParsedDoc);
                    _logger.LogInformation("Successfully updated ParsedDoc with ID: {Id}", @event.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing ParsedDocUpdatedEvent for ID: {Id}", @event.Id);
                throw new EventHandlerException(nameof(ParsedDocEventHandler), "ParsedDocUpdatedEvent error processing parsed document", ex);
            }
        }


        public async Task OnEvent(ParsedDocDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: ParsedDocDeletedEvent: {Id}", @event.Id);
            try
            {
                await _parsedDocRepository.Delete(@event.Id);
            }
            catch (Exception ex)
            {
                throw new EventHandlerException(nameof(ParsedDocEventHandler), "ParsedDocDeletedEvent Error deleting parsed document", ex);
            }
        }
    }
}