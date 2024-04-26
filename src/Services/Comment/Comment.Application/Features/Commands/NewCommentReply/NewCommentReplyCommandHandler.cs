using AutoMapper;
using CQRS.Core.Handlers;
using CQRS.Core.Exceptions;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Features.Commands.NewCommentReply
{
    public class NewCommentReplyCommandHandler : IRequestHandler<NewCommentReplyCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewCommentReplyCommandHandler> _logger;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IEventSourcingHandler<CommentAggregate> _commentEventSourcingHandler;

        public NewCommentReplyCommandHandler(ILogger<NewCommentReplyCommandHandler> logger,
            IEventSourcingHandler<CommentAggregate> commentEventSourcingHandler,
            ICommentReplyRepository commentReplyRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _commentEventSourcingHandler = commentEventSourcingHandler ?? throw new ArgumentNullException(nameof(commentEventSourcingHandler));
        }

        public async Task<Unit> Handle(NewCommentReplyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling NewCommentReplyCommand: {request}");

            request.DateCreated = DateTime.UtcNow;
            request.IsModified = false;

            try
            {
                var commentReplyAddedEvent = _mapper.Map<CommentReplyAddedEvent>(request);
                commentReplyAddedEvent.CreatedById = request.RequestorUserId;

                var aggregate = await _commentEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.AddCommentReply(commentReplyAddedEvent);

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