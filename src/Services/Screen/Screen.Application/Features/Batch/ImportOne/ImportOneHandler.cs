
using AutoMapper;
using Daikon.Shared.Constants.AppScreen;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Commands.NewHitCollection;
using Screen.Application.Features.Commands.NewScreen;
using Screen.Application.Features.Commands.NewScreenRun;

namespace Screen.Application.Features.Batch.ImportOne
{
    public class ImportOneHandler : IRequestHandler<ImportOneCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ImportOneHandler> _logger;
        private readonly IMediator _mediator;

        public ImportOneHandler(IMapper mapper, ILogger<ImportOneHandler> logger, IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ImportOneCommand request, CancellationToken cancellationToken)
        {
            // 1st create the screen
            _logger.LogInformation("ImportOneHandler {request}", request);

            var createScreenCommand = _mapper.Map<NewScreenCommand>(request);
            var screenId = createScreenCommand.Id;
            await _mediator.Send(createScreenCommand, cancellationToken);
            _logger.LogInformation("Screen created");

            // 2nd create the screen runs
            _logger.LogInformation("Creating ScreenRuns");
            foreach (var screenRun in request.ScreenRuns)
            {
                var createScreenRunCommand = _mapper.Map<NewScreenRunCommand>(screenRun);
                createScreenRunCommand.Id = screenId;
                createScreenRunCommand.ScreenRunId = screenRun.Id;
                
                await _mediator.Send(createScreenRunCommand, cancellationToken);
            }
            _logger.LogInformation("ScreenRuns created");

            // 3rd create the HitCollection
            var hitCollectionName = "";
            var hitCollectionId = Guid.NewGuid();
            var hitCollectionType = "";

            _logger.LogInformation("Creating HitCollection");
            if (request.Hits != null && request.Hits.Any())
            {
                if (request.ScreenType == ScreeningType.TargetBased)
                {
                    hitCollectionName = createScreenCommand.Name + "-V1";
                    hitCollectionType = "validated";

                }
                else if (request.ScreenType == ScreeningType.Phenotypic)
                {
                    hitCollectionName = createScreenCommand.Name + "-D1";
                    hitCollectionType = "disclosed";
                }
                else
                {
                    hitCollectionName = createScreenCommand.Name + "-O1";
                    hitCollectionType = "other";
                }
            }
            _logger.LogInformation("hitCollectionName: {hitCollectionName}", hitCollectionName);
            _logger.LogInformation("hitCollectionType: {hitCollectionType}", hitCollectionType);

            var createHitCollectionCommand = new NewHitCollectionCommand
            {
                Id = hitCollectionId,
                ScreenId = screenId,
                Name = hitCollectionName,
                HitCollectionType = hitCollectionType

            };

            await _mediator.Send(createHitCollectionCommand, cancellationToken);

            _logger.LogInformation("HitCollection created");


            // 4th create the hits
            _logger.LogInformation("Creating Hits");
            foreach (var hit in request.Hits)
            {
                var createHitCommand = _mapper.Map<NewHitCommand>(hit);
                createHitCommand.Id = hitCollectionId;
                createHitCommand.HitId = hit.Id;
                await _mediator.Send(createHitCommand, cancellationToken);
            }
            _logger.LogInformation("Hits created");
            return Unit.Value;
        }
    }

}