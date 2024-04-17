using AutoMapper;
using CQRS.Core.Exceptions;
using Comment.Application.Contracts.Persistence;
using MediatR;

namespace Comment.Application.Features.Queries.GetComment.ById
{
    public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, CommentVM>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IMapper _mapper;

        public GetCommentByIdQueryHandler(ICommentRepository commentRepository,ICommentReplyRepository commentReplyRepository,  IMapper mapper)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _commentReplyRepository = commentReplyRepository ?? throw new ArgumentNullException(nameof(commentReplyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CommentVM> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.ReadById(request.Id) ?? throw new ResourceNotFoundException(nameof(Comment), request.Id);

            var replies = await _commentReplyRepository.ListByCommentId(request.Id);

            var commentVm = _mapper.Map<CommentVM>(comment, opts => opts.Items["WithMeta"] = request.WithMeta);
            commentVm.Replies = _mapper.Map<List<CommentReplyVM>>(replies, opts => opts.Items["WithMeta"] = request.WithMeta);

            return commentVm;
        }

    
        
    }
}