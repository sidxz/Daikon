using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Application.Features.Commands.ApproveTarget;
using Target.Application.Features.Commands.SubmitTPQ;

namespace Target.Application.BatchOperations.BatchCommands.ImportOne
{
    public class ImportOneHandler : IRequestHandler<ImportOneCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<ImportOneHandler> _logger;
        private readonly IMediator _mediator;
        private readonly ITargetRepository _targetRepository;

        public ImportOneHandler(IMapper mapper, ILogger<ImportOneHandler> logger, IMediator mediator, ITargetRepository targetRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            _targetRepository = targetRepository;
        }

        public async Task<Unit> Handle(ImportOneCommand request, CancellationToken cancellationToken)
        {
            // check if the target already exists
            var existingTarget = await _targetRepository.ReadTargetByName(request.Name);
            if (existingTarget != null)
            {
                _logger.LogInformation($"Target with name {request.Name} already exists. Skipping import.");
                return Unit.Value;
            }

            //  1. Submit TPQ
            

            var tpqId = Guid.NewGuid();
            var tpqCommand = _mapper.Map<SubmitTPQCommand>(request);
            tpqCommand.Id = tpqId;
            tpqCommand.RequestedTargetName = request.Name;
            await _mediator.Send(tpqCommand, cancellationToken);
            _logger.LogInformation($"TPQ submitted with id {tpqId}");

            //  2. Approve Target
            var approveCommand = _mapper.Map<ApproveTargetCommand>(request);
            approveCommand.Id = request.Id;
            approveCommand.TPQId = tpqId;
            approveCommand.TargetName = request.Name;
            approveCommand.AssociatedGenes = request.RequestedAssociatedGenes;
            await _mediator.Send(approveCommand, cancellationToken);
            _logger.LogInformation($"Target approved with id {request.Id}");

            return Unit.Value;

        }
    }
}