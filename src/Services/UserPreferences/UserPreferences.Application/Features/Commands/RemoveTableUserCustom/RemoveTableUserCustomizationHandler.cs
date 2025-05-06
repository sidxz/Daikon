using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;

namespace UserPreferences.Application.Features.Commands.RemoveTableUserCustom
{
    /*
     * Handles the removal of user-specific table customization.
     * Safely checks if the customization exists before attempting deletion.
     * Gracefully handles errors and logs appropriate messages.
     */
    public class RemoveTableUserCustomizationHandler : IRequestHandler<RemoveTableUserCustomizationCommand>
    {
        private readonly ILogger<RemoveTableUserCustomizationHandler> _logger;
        private readonly ITableUserCustomizationRepository _repository;

        public RemoveTableUserCustomizationHandler(
            ILogger<RemoveTableUserCustomizationHandler> logger,
            ITableUserCustomizationRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Unit> Handle(RemoveTableUserCustomizationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to remove customization for UserId: {UserId}, TableType: {TableType}, TableInstanceId: {TableInstanceId}",
                request.RequestorUserId, request.TableType, request.TableInstanceId);

            try
            {
                // Check if customization exists for the given parameters
                var existingCustomization = await _repository.GetByUserAsync(
                    request.TableType,
                    request.TableInstanceId,
                    request.RequestorUserId);

                if (existingCustomization == null)
                {
                    _logger.LogWarning("No customization found for UserId: {UserId} on TableInstanceId: {TableInstanceId}", 
                        request.RequestorUserId, request.TableInstanceId);
                    return Unit.Value;
                }

                // Proceed to delete existing customization
                await _repository.DeleteAsync(
                    request.TableType,
                    request.TableInstanceId,
                    request.RequestorUserId);

                _logger.LogInformation("Successfully removed customization for UserId: {UserId} on TableInstanceId: {TableInstanceId}", 
                    request.RequestorUserId, request.TableInstanceId);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete customization for UserId: {UserId} on TableInstanceId: {TableInstanceId}",
                    request.RequestorUserId, request.TableInstanceId);

                throw new RepositoryException(
                    nameof(RemoveTableUserCustomizationHandler),
                    "An error occurred while deleting user customization.",
                    ex);
            }
        }
    }
}
