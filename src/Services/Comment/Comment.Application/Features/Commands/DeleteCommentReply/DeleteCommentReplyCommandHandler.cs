
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Comment.Domain.Aggregates;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;

namespace Comment.Application.Features.Commands.DeleteCommentReply
{
    public class DeleteCommentReplyCommandHandler : IRequestHandler<DeleteCommentReplyCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCommentReplyCommandHandler> _logger;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IEventSourcingHandler<CommentAggregate> _commentEventSourcingHandler;

        public DeleteCommentReplyCommandHandler(ILogger<DeleteCommentReplyCommandHandler> logger,
            IEventSourcingHandler<CommentAggregate> commentEventSourcingHandler,
            ICommentReplyRepository commentReplyRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _commentEventSourcingHandler = commentEventSourcingHandler ?? throw new ArgumentNullException(nameof(commentEventSourcingHandler));
        }

        public async Task<Unit> Handle(DeleteCommentReplyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling DeleteCommentReplyCommand: {request}");
            try
            {
                var commentReplyDeletedEvent = _mapper.Map<CommentReplyDeletedEvent>(request);

                var aggregate = await _commentEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.DeleteCommentReply(commentReplyDeletedEvent);

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