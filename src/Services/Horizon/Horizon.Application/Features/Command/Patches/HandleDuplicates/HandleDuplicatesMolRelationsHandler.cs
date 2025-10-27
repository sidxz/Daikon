
using Horizon.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Features.Command.Patches.HandleDuplicates
{
  public class HandleDuplicatesMolRelationsHandler : IRequestHandler<HandleDuplicatesMolRelationsCommand, Unit>
  {
    
    private readonly ILogger<HandleDuplicatesMolRelationsHandler> _logger;
    private readonly IGraphRepositoryForMLogix _graphRepository;

    public HandleDuplicatesMolRelationsHandler(ILogger<HandleDuplicatesMolRelationsHandler> logger, IGraphRepositoryForMLogix graphRepository)
    {
        _logger = logger;
        _graphRepository = graphRepository;
    }

    public async Task<Unit> Handle(HandleDuplicatesMolRelationsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling duplicate relations for Molecules");

        await _graphRepository.RemoveDuplicateRelationsAsync();

        return Unit.Value;
    }
  }
}