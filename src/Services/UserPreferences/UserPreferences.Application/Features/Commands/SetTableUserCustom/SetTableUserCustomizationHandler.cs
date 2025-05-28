using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Commands.SetTableUserCustom
{
    /*
     * Handles creation or update of user-specific table customization.
     * Ensures correct persistence of the configuration and sets audit metadata.
     */
    public class SetTableUserCustomizationHandler : IRequestHandler<SetTableUserCustomizationCommand, TableUserCustomization>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SetTableUserCustomizationHandler> _logger;
        private readonly ITableUserCustomizationRepository _repository;

        public SetTableUserCustomizationHandler(
            IMapper mapper,
            ILogger<SetTableUserCustomizationHandler> logger,
            ITableUserCustomizationRepository repository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<TableUserCustomization> Handle(SetTableUserCustomizationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing SetTableUserCustomizationCommand for UserId: {UserId}, TableInstanceId: {TableInstanceId}",
                request.RequestorUserId, request.TableInstanceId);

            // Reject if the TableType is Guid.Empty
            if (request.TableInstanceId == Guid.Empty)
            {
                _logger.LogWarning("TableType cannot be empty. Command rejected.");
                return null;
            }

            try
            {
                // Check if a customization already exists
                var existingCustomization = await _repository.GetByUserAsync(
                    request.TableType,
                    request.TableInstanceId,
                    request.RequestorUserId);

                if (existingCustomization != null)
                {
                    _logger.LogInformation("Existing customization found. Updating...");
                    request.SetUpdateProperties(request.RequestorUserId);
                    request.Id = existingCustomization.Id;
                }
                else
                {
                    _logger.LogInformation("No existing customization found. Creating new...");
                    request.SetCreateProperties(request.RequestorUserId);
                    request.Id = Guid.NewGuid();
                }

                // Map to domain entity
                var userCustomization = _mapper.Map<TableUserCustomization>(request);
                userCustomization.UserId = request.RequestorUserId;

                // Save or update the customization
                await _repository.ReplaceAsync(
                    request.TableType,
                    request.TableInstanceId,
                    request.RequestorUserId,
                    userCustomization);

                _logger.LogInformation("User customization saved successfully for UserId: {UserId}, TableInstanceId: {TableInstanceId}",
                    request.RequestorUserId, request.TableInstanceId);

                return userCustomization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user customization for UserId: {UserId}, TableInstanceId: {TableInstanceId}",
                    request.RequestorUserId, request.TableInstanceId);
                throw; // Optionally wrap in a custom application exception
            }
        }
    }
}
