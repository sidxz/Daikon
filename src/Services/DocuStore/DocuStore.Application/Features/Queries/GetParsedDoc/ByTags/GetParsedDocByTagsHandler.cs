using AutoMapper;
using Daikon.Shared.VM.DocuStore;
using DocuStore.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DocuStore.Application.Features.Queries.GetParsedDoc.ByTags
{
    public class GetParsedDocByTagsHandler : IRequestHandler<GetParsedDocByTagsQuery, List<ParsedDocVM>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetParsedDocByTagsHandler> _logger;
        private readonly IParsedDocRepository _parsedDocRepository;

        public GetParsedDocByTagsHandler(
            IMapper mapper,
            ILogger<GetParsedDocByTagsHandler> logger,
            IParsedDocRepository parsedDocRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
        }

        public async Task<List<ParsedDocVM>> Handle(GetParsedDocByTagsQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("Received a null request in {HandlerName}", nameof(GetParsedDocByTagsHandler));
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            if (request.Tags == null || !request.Tags.Any())
            {
                _logger.LogWarning("Received an empty or null tags list in {HandlerName}.", nameof(GetParsedDocByTagsHandler));
                throw new ArgumentException("Tags list cannot be null or empty.", nameof(request.Tags));
            }

            try
            {
                // Fetch documents matching the specified tags
                var parsedDocs = await _parsedDocRepository.ListByTags(request.Tags);

                if (parsedDocs == null || !parsedDocs.Any())
                {
                    _logger.LogInformation("No parsed documents found matching the specified tags: {Tags}.", string.Join(", ", request.Tags));
                    return new List<ParsedDocVM>(); // Return an empty list if no documents are found
                }

                // Log successful retrieval
                _logger.LogInformation("Successfully retrieved {DocumentCount} parsed documents matching the specified tags.", parsedDocs.Count);

                // Map the entities to ViewModel list
                var parsedDocsVM = _mapper.Map<List<ParsedDocVM>>(parsedDocs, opts =>
                {
                    opts.Items["WithMeta"] = request.WithMeta;
                });

                return parsedDocsVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while handling the request for parsed documents by tags: {Tags}.", string.Join(", ", request.Tags));
                throw new ApplicationException("An error occurred while processing the request. Please try again later.", ex);
            }
        }
    }
}
