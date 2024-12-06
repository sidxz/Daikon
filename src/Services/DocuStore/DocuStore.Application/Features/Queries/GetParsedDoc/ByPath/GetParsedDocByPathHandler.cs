
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Shared.VM.DocuStore;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DocuStore.Application.Features.Queries.GetParsedDoc.ByPath
{
    public class GetParsedDocByPathHandler : IRequestHandler<GetParsedDocByPathQuery, ParsedDocVM>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetParsedDocByPathHandler> _logger;
        private readonly IParsedDocRepository _parsedDocRepository;

        public GetParsedDocByPathHandler(
            IMapper mapper,
            ILogger<GetParsedDocByPathHandler> logger,
            IParsedDocRepository parsedDocRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
        }

        public async Task<ParsedDocVM> Handle(GetParsedDocByPathQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("Received a null request in {HandlerName}", nameof(GetParsedDocByPathHandler));
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            try
            {
                // Fetch the parsed document by ID
                var parsedDoc = await _parsedDocRepository.ReadByPath(request.Path);

                if (parsedDoc == null)
                {
                    _logger.LogWarning("Parsed document with Path {path} was not found.", request.Path);
                    throw new ResourceNotFoundException(nameof(ParsedDoc), request.Path);
                }

                // Log successful retrieval
                _logger.LogInformation("Successfully retrieved parsed document with Path, ID : {Path}, {DocumentId}.", request.Path, parsedDoc.Id);

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
                _logger.LogError(ex, "An unexpected error occurred while handling the request for parsed document ID {Path}.", request.Path);
                throw new ApplicationException("An error occurred while processing the request. Please try again later.", ex);
            }
        }
    }
}
