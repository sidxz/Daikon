
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Application.Features.Command.NewTarget;
using Target.Domain.Aggregates;
using Daikon.Shared.Constants.AppTarget;
using Daikon.Events.Targets;

namespace Target.Application.Features.Commands.ApproveTarget
{
    public class ApproveTargetHandler : IRequestHandler<ApproveTargetCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<ApproveTargetHandler> _logger;
        private readonly IEventSourcingHandler<TargetAggregate> _targetESH;
        private readonly IEventSourcingHandler<TPQuestionnaireAggregate> _questionnaireESH;
        private readonly ITargetRepository _targetRepository;
        private readonly IPQResponseRepository _pQResponseRepository;
        private readonly IMediator _mediator;

        public ApproveTargetHandler(IMapper mapper, ILogger<ApproveTargetHandler> logger, IEventSourcingHandler<TargetAggregate> targetESH, IEventSourcingHandler<TPQuestionnaireAggregate> questionnaireESH, ITargetRepository targetRepository, IPQResponseRepository pQResponseRepository, IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _targetESH = targetESH;
            _questionnaireESH = questionnaireESH;
            _targetRepository = targetRepository;
            _pQResponseRepository = pQResponseRepository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ApproveTargetCommand request, CancellationToken cancellationToken)
        {
            // check if target (targetName) already exists within same strain ; reject if it does
            var existingTarget = await _targetRepository.ReadTargetByName(request.TargetName);
            if (existingTarget!= null && 
                (existingTarget.Name == request.TargetName && existingTarget.StrainId == request.StrainId))
            {
                throw new DuplicateEntityRequestException(nameof(ApproveTargetCommand), request.TargetName);
            }

            // Now check if the TPQ has been submitted
            var tpq = await _pQResponseRepository.ReadById(request.TPQId);
            if (tpq == null)
            {
                throw new AggregateNotFoundException(nameof(ApproveTargetCommand));
            }

            // Create the target
            // var newTargetCommand = new NewTargetCommand
            // {
            //     Id = request.Id,
            //     StrainId = request.StrainId,
            //     Name = request.TargetName,
            //     AssociatedGenes = request.AssociatedGenes,
            //     TargetType = request.AssociatedGenes.Count > 1 ? TargetType.ProteinComplex : TargetType.Protein  
            // };

            var newTargetCommand = _mapper.Map<NewTargetCommand>(request);
            newTargetCommand.Id = request.Id;
            newTargetCommand.StrainId = request.StrainId;
            newTargetCommand.Name = request.TargetName;
            newTargetCommand.AssociatedGenes = request.AssociatedGenes;
            newTargetCommand.TargetType = request.AssociatedGenes.Count > 1 ? TargetType.ProteinComplex : TargetType.Protein;


            await _mediator.Send(newTargetCommand, cancellationToken);
            _logger.LogInformation(" +++++++++++++++++ New Target created with Name {TargetName}", request.TargetName);

            // now update the TPQ to reflect the new target

            var tpqUpdatedEvent = _mapper.Map<TargetPromotionQuestionnaireUpdatedEvent>(tpq);
            tpqUpdatedEvent.TargetId = newTargetCommand.Id;
            tpqUpdatedEvent.ApprovedTargetName = request.TargetName;
            tpqUpdatedEvent.ApprovedAssociatedGenes = request.AssociatedGenes;
            tpqUpdatedEvent.IsVerified = true;
            tpqUpdatedEvent.VerifiedBy = request.RequestorUserId.ToString();

            try
            {
                var aggregate = await _questionnaireESH.GetByAsyncId(request.TPQId);

                aggregate.UpdatePQResponse(tpqUpdatedEvent);

                await _questionnaireESH.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogError(ex, "Aggregate not found");
            }

            _logger.LogInformation(" +++++++++++++++++ TPQ updated with new target");

            return Unit.Value;


            throw new NotImplementedException();
        }
    }
}