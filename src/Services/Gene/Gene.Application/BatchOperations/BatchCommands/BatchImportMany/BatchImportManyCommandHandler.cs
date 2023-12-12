
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportMany
{
    public class BatchImportManyCommandHandler : IRequestHandler<BatchImportManyCommand, Unit>
    {


        private readonly ILogger<BatchImportManyCommandHandler> _logger;
        private readonly IMediator _mediator;

        public BatchImportManyCommandHandler(ILogger<BatchImportManyCommandHandler> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(BatchImportManyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("BatchImportManyCommandHandler.Handle");
            var batch = request.Batch;
            var tasks = new List<Task>();
            foreach (var batchImportOneCommand in batch)
            {
                _logger.LogDebug("BatchImportManyCommandHandler.Handle: Importing {batchImportOneCommandAccession}", batchImportOneCommand.AccessionNumber);
                tasks.Add(_mediator.Send(batchImportOneCommand, cancellationToken));
            }

            await Task.WhenAll(tasks);
            return Unit.Value;
        }





    }

}