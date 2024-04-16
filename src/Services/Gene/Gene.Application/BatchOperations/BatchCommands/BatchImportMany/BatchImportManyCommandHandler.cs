using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Gene.Application.BatchOperations.BatchCommands.BatchImportOne;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportMany
{
    public class BatchImportManyCommandHandler : IRequestHandler<BatchImportManyCommand, string>
    {
        private readonly ILogger<BatchImportManyCommandHandler> _logger;
        private readonly IMediator _mediator;

        public BatchImportManyCommandHandler(ILogger<BatchImportManyCommandHandler> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task<string> Handle(BatchImportManyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting background batch import");

            // Running the batch import in a background task
            Task.Run(() => ProcessBatch(request, cancellationToken), cancellationToken);

            // Immediately return success message
            return Task.FromResult("Batch import started successfully.");
        }

        private async Task ProcessBatch(BatchImportManyCommand request, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var command in request.Batch)
            {
                // Encapsulate task execution in a local async function to independently catch exceptions per task
                tasks.Add(RunIndividualCommand(command, cancellationToken));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
            _logger.LogInformation("All batch imports processed, with individual success or failure.");
        }

        private async Task RunIndividualCommand(BatchImportOneCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Processing {AccessionNumber}", command.AccessionNumber);
                await _mediator.Send(command, cancellationToken);
                _logger.LogInformation("Successfully processed {AccessionNumber}", command.AccessionNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process {AccessionNumber}", command.AccessionNumber);
                // Optionally handle or rethrow the exception if you need to escalate failure
            }
        }
    }
}
