using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using CQRS.Core.Domain;

namespace Comment.Application.Features.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCommentCommandHandler> _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IEventSourcingHandler<CommentAggregate> _commentEventSourcingHandler;

        public UpdateCommentCommandHandler(ILogger<UpdateCommentCommandHandler> logger,
            IEventSourcingHandler<CommentAggregate> commentEventSourcingHandler,
            ICommentRepository commentRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _commentEventSourcingHandler = commentEventSourcingHandler ?? throw new ArgumentNullException(nameof(commentEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Handling UpdateCommentCommand: {request}");

            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;

            request.Tags ??= [];
            request.Mentions ??= [];
            request.Subscribers ??= [];

            request.Description ??= new DVariable<string>(string.Empty);
            request.IsCommentLocked ??= false;


            var commentUpdatedEvent = _mapper.Map<CommentUpdatedEvent>(request);
            commentUpdatedEvent.LastModifiedById = request.RequestorUserId;

            try
            {
                var aggregate = await _commentEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateComment(commentUpdatedEvent);

                await _commentEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(CommentAggregate), request.Id); ;
            }

            return Unit.Value;
        }
    }
}