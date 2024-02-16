using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Features.Commands.NewComment
{
    public class NewCommentCommandHandler : IRequestHandler<NewCommentCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewCommentCommandHandler> _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IEventSourcingHandler<CommentAggregate> _commentEventSourcingHandler;

        public NewCommentCommandHandler(ILogger<NewCommentCommandHandler> logger,
            IEventSourcingHandler<CommentAggregate> commentEventSourcingHandler,
            ICommentRepository commentRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _commentEventSourcingHandler = commentEventSourcingHandler ?? throw new ArgumentNullException(nameof(commentEventSourcingHandler));
        }

        public async Task<Unit> Handle(NewCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Handling NewCommentCommand: {request}");

                // check if Id exists
                var existingComment = await _commentRepository.ReadCommentById(request.Id);
                if (existingComment != null)
                {
                    _logger.LogWarning("Comment Id already exists: {Id}", request.Id);
                    throw new InvalidOperationException("Comment Id already exists");
                }

                var newCommentCreatedEvent = _mapper.Map<CommentCreatedEvent>(request);

                var aggregate = new CommentAggregate(newCommentCreatedEvent);

                await _commentEventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling NewCommentCommand");
                throw;
            }
        }
        
    }
}