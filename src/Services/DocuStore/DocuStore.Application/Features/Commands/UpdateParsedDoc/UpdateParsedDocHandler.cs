using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.DocuStore;
using Daikon.Shared.APIClients.MLogix;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Application.Features.Commands.AddParsedDoc;
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
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly IMediator _mediator;

        public UpdateParsedDocHandler(
            IMapper mapper,
            ILogger<UpdateParsedDocHandler> logger,
            IParsedDocRepository parsedDocRepository,
            IEventSourcingHandler<ParsedDocAggregate> parsedDocEventSourcingHandler,
            IMLogixAPI mLogixAPIService,
            IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsedDocRepository = parsedDocRepository ?? throw new ArgumentNullException(nameof(parsedDocRepository));
            _parsedDocEventSourcingHandler = parsedDocEventSourcingHandler ?? throw new ArgumentNullException(nameof(parsedDocEventSourcingHandler));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ParsedDoc> Handle(UpdateParsedDocCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FilePath))
            {
                _logger.LogWarning("FilePath is missing or empty.");
                throw new InvalidOperationException("FilePath is required.");
            }

            try
            {
                var existingDoc = await _parsedDocRepository.ReadByPath(request.FilePath);
                if (existingDoc == null)
                {
                    _logger.LogWarning("Document does not exist: {FilePath}. Creating a new one.", request.FilePath);
                    return await CreateNewParsedDoc(request, cancellationToken);
                }

                request.SetUpdateProperties(request.RequestorUserId);
                request.Id = existingDoc.Id;

                await HandleSmilesExtraction(request);
                UpdateReviews(request, existingDoc);
                UpdateRatings(request, existingDoc);


                var parsedDocUpdatedEvent = _mapper.Map<ParsedDocUpdatedEvent>(request);
                var aggregate = await _parsedDocEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateParsedDoc(parsedDocUpdatedEvent);
                await _parsedDocEventSourcingHandler.SaveAsync(aggregate);

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
                _logger.LogError(ex, "Unexpected error while processing UpdateParsedDocCommand.");
                throw new ApplicationException("An error occurred while updating the parsed document.", ex);
            }
        }

        private async Task<ParsedDoc> CreateNewParsedDoc(UpdateParsedDocCommand request, CancellationToken cancellationToken)
        {
            var addParsedDocCommand = _mapper.Map<AddParsedDocCommand>(request);
            addParsedDocCommand.SetCreateProperties(request.RequestorUserId);
            addParsedDocCommand.Id = addParsedDocCommand.Id != Guid.Empty
                ? addParsedDocCommand.Id
                : Guid.NewGuid();

            return await _mediator.Send(addParsedDocCommand, cancellationToken);
        }

        private async Task HandleSmilesExtraction(UpdateParsedDocCommand request)
        {
            var uniqueSmiles = request.ExtractedSMILES?.Distinct() ?? Enumerable.Empty<string>();
            var existingTags = new HashSet<string>(request.Tags ?? []);

            foreach (var smiles in uniqueSmiles)
            {
                var molecule = await _mLogixAPIService.GetMoleculeBySmiles(smiles);
                if (molecule != null)
                {
                    var moleculeId = molecule.Id.ToString();
                    request.Molecules.TryAdd(moleculeId, molecule.Name);

                    if (existingTags.Add(molecule.Name))
                    {
                        request.Tags.Add(molecule.Name);
                    }
                }
            }
        }

        private void UpdateReviews(UpdateParsedDocCommand request, ParsedDoc existingDoc)
        {
            var requestorId = request.RequestorUserId;

            // Preserve all reviews from other users
            var updatedReviews = existingDoc.Reviews
                .Where(r => r.Reviewer != requestorId)
                .ToList();

            // Map of existing user reviews for fast lookup
            var existingUserReviews = existingDoc.Reviews
                .Where(r => r.Reviewer == requestorId)
                .ToDictionary(r => r.Id, r => r);

            // Filter request to only include reviews from current user
            var userSubmittedReviews = request.Reviews
                .Where(r => r.Reviewer == requestorId)
                .ToList();

            foreach (var submittedReview in userSubmittedReviews)
            {
                if (submittedReview.Id != Guid.Empty && existingUserReviews.TryGetValue(submittedReview.Id, out var existingReview))
                {
                    // Update existing review
                    existingReview.Review = submittedReview.Review;
                    existingReview.ReviewDate = DateTime.UtcNow;
                    updatedReviews.Add(existingReview);
                }
                else
                {
                    // Add new review
                    submittedReview.Id = Guid.NewGuid();
                    submittedReview.Reviewer = requestorId;
                    submittedReview.ReviewDate = DateTime.UtcNow;
                    updatedReviews.Add(submittedReview);
                }
            }

            request.Reviews = updatedReviews;
        }


        private void UpdateRatings(UpdateParsedDocCommand request, ParsedDoc existingDoc)
        {
            var requestorId = request.RequestorUserId;

            // Start with all ratings from other users
            var updatedRatings = existingDoc.Ratings
                .Where(r => r.UserId != requestorId)
                .ToList();

            // Get the submitted rating from the current user (only 1 should exist)
            var submittedRating = request.Ratings
                .FirstOrDefault(r => r.UserId == requestorId);

            if (submittedRating != null)
            {
                // Ensure the rating has a valid ID
                if (submittedRating.Id == Guid.Empty)
                {
                    submittedRating.Id = Guid.NewGuid();
                }

                // Validate the score range
                if (submittedRating.Score < 0 || submittedRating.Score > 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(submittedRating.Score), "Score must be between 0 and 5.");
                }

                // Check if the user has previously rated this doc
                var existingUserRating = existingDoc.Ratings
                    .FirstOrDefault(r => r.UserId == requestorId);

                if (existingUserRating != null)
                {
                    // Update the existing rating
                    existingUserRating.Score = submittedRating.Score;
                    existingUserRating.Comment = submittedRating.Comment;
                    existingUserRating.RatedAt = DateTime.UtcNow;

                    updatedRatings.Add(existingUserRating);
                }
                else
                {
                    // Add the new rating
                    submittedRating.UserId = requestorId; // Enforce correct user
                    submittedRating.RatedAt = DateTime.UtcNow;

                    updatedRatings.Add(submittedRating);
                }
            }

            // Set the final trusted rating list
            request.Ratings = updatedRatings;
        }


    }
}
