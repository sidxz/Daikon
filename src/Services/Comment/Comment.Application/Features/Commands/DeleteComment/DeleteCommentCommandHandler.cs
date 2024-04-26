
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Comment.Domain.Aggregates;
using Daikon.Events.Comment;

namespace Comment.Application.Features.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
    {
        private readonly ILogger<DeleteCommentCommandHandler> _logger;
        private readonly IEventSourcingHandler<CommentAggregate> _commentEventSourcingHandler;
        private readonly IMapper _mapper;

        public DeleteCommentCommandHandler(ILogger<DeleteCommentCommandHandler> logger,
        IEventSourcingHandler<CommentAggregate> commentEventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _commentEventSourcingHandler = commentEventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling DeleteCommentCommand: {request}");
            
            try
            {
                var aggregate = await _commentEventSourcingHandler.GetByAsyncId(request.Id);

                var commentDeletedEvent = _mapper.Map<CommentDeletedEvent>(request);

                aggregate.DeleteComment(commentDeletedEvent);

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