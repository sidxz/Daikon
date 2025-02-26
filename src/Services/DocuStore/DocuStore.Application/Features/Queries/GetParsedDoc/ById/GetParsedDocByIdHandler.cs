
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Shared.VM.DocuStore;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DocuStore.Application.Features.Queries.GetParsedDoc.ById
{
    public class GetParsedDocByIdHandler : IRequestHandler<GetParsedDocByIdQuery, ParsedDocVM>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetParsedDocByIdHandler> _logger;
        private readonly IParsedDocRepository _parsedDocRepository;

        public GetParsedDocByIdHandler(
            IMapper mapper,
            ILogger<GetParsedDocByIdHandler> logger,
            IParsedDocRepository parsedDocRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
        }

        public async Task<ParsedDocVM> Handle(GetParsedDocByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("Received a null request in {HandlerName}", nameof(GetParsedDocByIdHandler));
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            try
            {
                // Fetch the parsed document by ID
                var parsedDoc = await _parsedDocRepository.ReadById(request.Id);

                if (parsedDoc == null)
                {
                    _logger.LogWarning("Parsed document with ID {DocumentId} was not found.", request.Id);
                    throw new ResourceNotFoundException(nameof(ParsedDoc), request.Id);
                }

                // Log successful retrieval
                _logger.LogInformation("Successfully retrieved parsed document with ID {DocumentId}.", request.Id);

                // Map the entity to the ViewModel
                var parsedDocVM = _mapper.Map<ParsedDocVM>(parsedDoc, opts =>
                {
                    opts.Items["WithMeta"] = request.WithMeta;
                });

                return parsedDocVM;
            }
            catch (ResourceNotFoundException)
            {
                // Rethrow known exceptions for upstream handling
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while handling the request for parsed document ID {DocumentId}.", request.Id);
                throw new ApplicationException("An error occurred while processing the request. Please try again later.", ex);
            }
        }
    }
}
