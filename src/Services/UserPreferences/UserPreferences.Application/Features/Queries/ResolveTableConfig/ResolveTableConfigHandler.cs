using MediatR;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UserPreferences.Application.Features.Queries.ResolveTableConfig
{
    public class ResolveTableConfigHandler : IRequestHandler<ResolveTableConfigQuery, ResolveTableConfigVM>
    {
        private readonly ITableUserCustomizationRepository _userCustomizationRepository;
        private readonly ITableGlobalConfigRepository _globalConfigRepository;
        private readonly ITableDefaultsRepository _defaultsRepository;
        private readonly ILogger<ResolveTableConfigHandler> _logger;

        public ResolveTableConfigHandler(
            ITableUserCustomizationRepository userCustomizationRepository,
            ITableGlobalConfigRepository globalConfigRepository,
            ITableDefaultsRepository defaultsRepository,
            ILogger<ResolveTableConfigHandler> logger)
        {
            _userCustomizationRepository = userCustomizationRepository;
            _globalConfigRepository = globalConfigRepository;
            _defaultsRepository = defaultsRepository;
            _logger = logger;
        }

        public async Task<ResolveTableConfigVM> Handle(ResolveTableConfigQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Resolving table configuration for user {UserId}, table type {TableType}, instance {TableInstanceId}",
                    request.RequestorUserId, request.TableType, request.TableInstanceId);

                var userConfigTask = _userCustomizationRepository.GetByUserAsync(request.TableType, request.TableInstanceId, request.RequestorUserId);
                var globalConfigTask = _globalConfigRepository.GetByTableInstanceAsync(request.TableType, request.TableInstanceId);
                var defaultConfigTask = _defaultsRepository.GetByTableTypeAsync(request.TableType);

                await Task.WhenAll(userConfigTask, globalConfigTask, defaultConfigTask);

                var userConfig = userConfigTask.Result;
                var globalConfig = globalConfigTask.Result;
                var defaultConfig = defaultConfigTask.Result;

                int currentVersion = globalConfig?.Version ?? defaultConfig?.Version ?? 1;

                if (userConfig != null && userConfig.Version == currentVersion)
                {
                    _logger.LogInformation("User-level config found and matched for user {UserId}", request.RequestorUserId);

                    return new ResolveTableConfigVM
                    {
                        Level = "User",
                        TableType = request.TableType,
                        TableInstanceId = request.TableInstanceId,
                        UserId = request.RequestorUserId,
                        Columns = userConfig.Columns,
                        Version = userConfig.Version
                    };
                }

                if (globalConfig != null)
                {
                    _logger.LogInformation("Global-level config used for table instance {TableInstanceId}", request.TableInstanceId);

                    return new ResolveTableConfigVM
                    {
                        Level = "Global",
                        TableType = request.TableType,
                        TableInstanceId = request.TableInstanceId,
                        UserId = globalConfig.LastModifiedById ?? globalConfig.CreatedById,
                        Columns = globalConfig.Columns,
                        Version = globalConfig.Version
                    };
                }

                _logger.LogWarning("Falling back to default config for table type {TableType}", request.TableType);

                return new ResolveTableConfigVM
                {
                    Level = "Defaults",
                    TableType = request.TableType,
                    TableInstanceId = request.TableInstanceId,
                    UserId = request.RequestorUserId,
                    Columns = defaultConfig?.Columns ?? [],
                    Version = defaultConfig?.Version ?? 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while resolving table configuration for user {UserId}", request.RequestorUserId);
                throw new ApplicationException("Failed to resolve table configuration. Please contact support.");
            }
        }
    }
}
