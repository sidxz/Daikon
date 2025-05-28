using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Application.Features.Commands.RemoveTableUserCustom;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Commands.SetTableGlobal
{
    /* 
     * Handles setting or updating a global configuration for a table instance.
     * Ensures that if a configuration exists, it's updated; otherwise, a new one is created.
     * Also removes any user-specific customizations tied to the table instance.
     */
    public class SetTableGlobalConfigHandler : IRequestHandler<SetTableGlobalConfigCommand, TableGlobalConfig>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SetTableGlobalConfigHandler> _logger;
        private readonly ITableGlobalConfigRepository _repository;
        private readonly IMediator _mediator;

        public SetTableGlobalConfigHandler(
            IMapper mapper,
            ILogger<SetTableGlobalConfigHandler> logger,
            ITableGlobalConfigRepository repository,
            IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<TableGlobalConfig> Handle(SetTableGlobalConfigCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing SetTableGlobalConfigCommand for TableType: {TableType}, TableInstanceId: {TableInstanceId}",
                    request.TableType, request.TableInstanceId);

                if (request.TableInstanceId == Guid.Empty)
                {
                    _logger.LogWarning("TableType cannot be empty. Command rejected.");
                    return null;
                }

                // Attempt to retrieve existing configuration
                var existingConfig = await _repository.GetByTableInstanceAsync(request.TableType, request.TableInstanceId);

                if (existingConfig != null)
                {
                    // Update properties if config exists
                    _logger.LogInformation("Existing config found. Updating configuration...");
                    request.SetUpdateProperties(request.RequestorUserId);
                    request.Id = existingConfig.Id;
                }
                else
                {
                    // Set create properties for new config
                    _logger.LogInformation("No existing config found. Creating new configuration...");
                    request.SetCreateProperties(request.RequestorUserId);
                    request.Id = Guid.NewGuid();
                }

                // Map request to domain entity
                var globalConfigEntity = _mapper.Map<TableGlobalConfig>(request);

                // Remove user-specific customizations
                await _mediator.Send(new RemoveTableUserCustomizationCommand
                {
                    TableType = request.TableType,
                    TableInstanceId = request.TableInstanceId,
                    RequestorUserId = request.RequestorUserId
                }, cancellationToken);

                // Save updated or new configuration
                await _repository.ReplaceAsync(request.TableType, request.TableInstanceId, globalConfigEntity);

                _logger.LogInformation("Global configuration successfully saved for TableInstanceId: {TableInstanceId}", request.TableInstanceId);

                return globalConfigEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling SetTableGlobalConfigCommand for TableInstanceId: {TableInstanceId}",
                    request.TableInstanceId);
                throw;
            }
        }
    }
}
