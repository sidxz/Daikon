using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;

namespace Screen.Application.Features.Commands.DeleteScreen
{
  /*
   * Handles the deletion of a Screen aggregate.
   * Prevents deletion if associated HitCollections exist.
   * Applies domain event (ScreenDeletedEvent) and persists changes via Event Sourcing.
   */
  public class DeleteScreenCommandHandler : IRequestHandler<DeleteScreenCommand, Unit>
  {
    private readonly ILogger<DeleteScreenCommandHandler> _logger;
    private readonly IEventSourcingHandler<ScreenAggregate> _screenEventHandler;
    private readonly IHitCollectionRepository _hitCollectionRepository;
    private readonly IMapper _mapper;

    public DeleteScreenCommandHandler(
        ILogger<DeleteScreenCommandHandler> logger,
        IEventSourcingHandler<ScreenAggregate> screenEventHandler,
        IHitCollectionRepository hitCollectionRepository,
        IMapper mapper)
    {
      _logger = logger;
      _screenEventHandler = screenEventHandler;
      _hitCollectionRepository = hitCollectionRepository;
      _mapper = mapper;
    }

    /*
     * Handles the deletion command for a Screen.
     * Throws ResourceCannotBeDeletedException if HitCollections exist.
     * Throws ResourceNotFoundException if Screen aggregate is missing.
     */
    public async Task<Unit> Handle(DeleteScreenCommand request, CancellationToken cancellationToken)
    {
      // Ensure audit info is applied to the command
      request.SetUpdateProperties(request.RequestorUserId);

      // Validate screen ID
      if (request.Id == Guid.Empty)
      {
        _logger.LogWarning("DeleteScreenCommand received with empty ScreenId.");
        throw new ArgumentException("Screen ID cannot be empty.", nameof(request.Id));
      }

      // Step 1: Ensure no dependent HitCollections exist
      try
      {
        var associatedHitCollections = await _hitCollectionRepository.GetHitCollectionsListByScreenId(request.Id);
        int hitCollectionCount = associatedHitCollections?.Count ?? 0;

        if (hitCollectionCount > 0)
        {
          _logger.LogWarning(
              "Attempted to delete Screen (Id: {ScreenId}), but {Count} associated HitCollections exist.",
              request.Id, hitCollectionCount);

          throw new ResourceCannotBeDeletedException(
              resourceName: nameof(ScreenAggregate),
              id: request.Id,
              message: "Screen has associated HitCollections and cannot be deleted.");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to retrieve HitCollections for ScreenId: {ScreenId}", request.Id);
        throw new ApplicationException("Failed to validate associated resources before deletion.", ex);
      }

      // Step 2: Fetch and delete the Screen aggregate
      try
      {
        var screenAggregate = await _screenEventHandler.GetByAsyncId(request.Id);

        var screenDeletedEvent = _mapper.Map<ScreenDeletedEvent>(request);
        screenAggregate.DeleteScreen(screenDeletedEvent);

        await _screenEventHandler.SaveAsync(screenAggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "ScreenAggregate not found for deletion. ScreenId: {ScreenId}", request.Id);
        throw new ResourceNotFoundException(resourceName: nameof(ScreenAggregate), id: request.Id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unexpected error occurred while deleting ScreenAggregate with Id: {ScreenId}", request.Id);
        throw new ApplicationException("An unexpected error occurred during Screen deletion.", ex);
      }

      // Successfully handled the command
      return Unit.Value;
    }
  }
}
