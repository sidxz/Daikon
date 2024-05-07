using AutoMapper;
using CQRS.Core.Exceptions;
using Comment.Application.Contracts.Persistence;
using MediatR;
using Comment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Comment.Application.Features.Queries.GetComment.ByTags
{
    public class GetCommentsByTagsHandler : IRequestHandler<GetCommentsByTagsQuery, List<CommentVM>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentsByTagsHandler> _logger;

        public GetCommentsByTagsHandler(ICommentRepository commentRepository,
        ICommentReplyRepository commentReplyRepository, IMapper mapper, ILogger<GetCommentsByTagsHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<List<CommentVM>> Handle(GetCommentsByTagsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var requestedTags = new List<string>(request.Tags);

                var comments = await _commentRepository.ListByTags(requestedTags);
                var commentVms = _mapper.Map<List<CommentVM>>(comments, opts => opts.Items["WithMeta"] = request.WithMeta);

                var replyTasks = commentVms.Select(async commentVm =>
                {
                    var replies = await _commentReplyRepository.ListByCommentId(commentVm.Id);
                    commentVm.Replies = _mapper.Map<List<CommentReplyVM>>(replies, opts => opts.Items["WithMeta"] = request.WithMeta);
                });

                await Task.WhenAll(replyTasks);
                
                return commentVms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }


    }
}