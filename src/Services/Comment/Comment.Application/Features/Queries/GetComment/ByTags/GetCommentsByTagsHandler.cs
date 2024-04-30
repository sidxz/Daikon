using AutoMapper;
using CQRS.Core.Exceptions;
using Comment.Application.Contracts.Persistence;
using MediatR;
using Comment.Domain.Entities;

namespace Comment.Application.Features.Queries.GetComment.ByTags
{
    public class GetCommentsByTagsHandler : IRequestHandler<GetCommentsByTagsQuery, List<CommentVM>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IMapper _mapper;

        public GetCommentsByTagsHandler(ICommentRepository commentRepository, ICommentReplyRepository commentReplyRepository, IMapper mapper)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<CommentVM>> Handle(GetCommentsByTagsQuery request, CancellationToken cancellationToken)
        {
            var requestedTags = new List<string>(request.Tags);

            var comments = await _commentRepository.ListByTags(requestedTags);

            var commentVms = _mapper.Map<List<CommentVM>>(comments, opts => opts.Items["WithMeta"] = request.WithMeta);

            commentVms.ForEach(async commentVm =>
            {
                var replies = await _commentReplyRepository.ListByCommentId(commentVm.Id);
                if (replies == null || replies.Count == 0)
                {
                    replies = new List<CommentReply>();
                }
                commentVm.Replies = _mapper.Map<List<CommentReplyVM>>(replies, opts => opts.Items["WithMeta"] = request.WithMeta);
            });

            return commentVms;
        }

    }
}