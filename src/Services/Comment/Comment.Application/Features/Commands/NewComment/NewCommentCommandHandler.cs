using AutoMapper;
using Daikon.EventStore.Handlers;
using Daikon.Events.Comment;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using CQRS.Core.Domain;

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
                
                request.DateCreated = DateTime.UtcNow;
                request.IsModified = false;

                request.Tags ??= [];
                request.Mentions ??= [];
                request.Subscribers ??= [];

                request.Description ??= new DVariable<string>(string.Empty);
                request.IsCommentLocked ??= false;

                var commentCreatedEvent = _mapper.Map<CommentCreatedEvent>(request);
                commentCreatedEvent.CreatedById = request.RequestorUserId;

                var aggregate = new CommentAggregate(commentCreatedEvent);

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