using AutoMapper;
using CQRS.Core.Exceptions;
using Comment.Application.Contracts.Persistence;
using MediatR;
using Comment.Domain.Entities;
using Microsoft.Extensions.Logging;
using Comment.Application.Features.Queries.GetComment;

namespace Comment.Application.Features.Queries.GetMostRecent
{
    public class GetMostRecentHandler : IRequestHandler<GetMostRecentQuery, List<CommentVM>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMostRecentHandler> _logger;

        public GetMostRecentHandler(ICommentRepository commentRepository,
        ICommentReplyRepository commentReplyRepository, IMapper mapper, ILogger<GetMostRecentHandler> logger)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<List<CommentVM>> Handle(GetMostRecentQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var comments = await _commentRepository.ListMostRecent(request.Count);
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