using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Features.Commands.UpdateCommentReply
{
    public class UpdateCommentReplyCommandHandler : IRequestHandler<UpdateCommentReplyCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCommentReplyCommandHandler> _logger;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IEventSourcingHandler<CommentAggregate> _commentEventSourcingHandler;

        public UpdateCommentReplyCommandHandler(ILogger<UpdateCommentReplyCommandHandler> logger,
            IEventSourcingHandler<CommentAggregate> commentEventSourcingHandler,
            ICommentReplyRepository commentReplyRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _commentEventSourcingHandler = commentEventSourcingHandler ?? throw new ArgumentNullException(nameof(commentEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateCommentReplyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling UpdateCommentReplyCommand: {request}");

            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;

            try
            {
                var commentReplyUpdatedEvent = _mapper.Map<CommentReplyUpdatedEvent>(request);
                commentReplyUpdatedEvent.LastModifiedById = request.RequestorUserId;

                var aggregate = await _commentEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateCommentReply(commentReplyUpdatedEvent);

                await _commentEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(CommentAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}