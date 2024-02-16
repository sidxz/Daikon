using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;
using Comment.Application.Features.Commands.UpdateComment;
using Comment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

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
            var commentUpdatedEvent = _mapper.Map<CommentUpdatedEvent>(request);

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

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling UpdateCommentCommandHandler");
                throw;
            }

            return Unit.Value;
        }
    }
}