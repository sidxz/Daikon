using MediatR;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Domain.Table;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UserPreferences.Application.Features.Queries.GetDefaults
{
    /*
     * Handler for retrieving default table settings from the repository.
     * Implements error handling and logging for maintainability and traceability.
     */
    public class GetDefaultsHandler : IRequestHandler<GetDefaultsQuery, TableDefaults>
    {
        private readonly ITableDefaultsRepository _tableDefaultsRepository;
        private readonly ILogger<GetDefaultsHandler> _logger;

        /*
         * Constructor to inject repository and logger dependencies.
         */
        public GetDefaultsHandler(
            ITableDefaultsRepository tableDefaultsRepository,
            ILogger<GetDefaultsHandler> logger)
        {
            _tableDefaultsRepository = tableDefaultsRepository ??
                throw new ArgumentNullException(nameof(tableDefaultsRepository));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        /*
         * Handles the GetDefaultsQuery request to retrieve table defaults.
         * Logs the operation and handles potential exceptions with meaningful output.
         */
        public async Task<TableDefaults> Handle(GetDefaultsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start processing GetDefaultsQuery to fetch all table defaults.");

            try
            {
                var tableDefaults = await _tableDefaultsRepository.GetByTableTypeAsync(request.TableType);

                _logger.LogInformation("Successfully retrieved table defaults.");
                return tableDefaults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while fetching table defaults.");
                throw new ApplicationException("An error occurred while retrieving table defaults.", ex);
            }
        }
    }
}
