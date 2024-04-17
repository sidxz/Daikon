using AutoMapper;
using MediatR;
using CQRS.Core.Exceptions;
using Comment.Application.Contracts.Persistence;

namespace Comment.Application.Features.Queries.GetCommentList
{
    public class GetCommentListQueryHandler : IRequestHandler<GetCommentListQuery, List<CommentListVM>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public GetCommentListQueryHandler(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<CommentListVM>> Handle(GetCommentListQuery request, CancellationToken cancellationToken)
        {
            var allComments = (await _commentRepository.ListAll()).OrderBy(x => x.Topic);
            return _mapper.Map<List<CommentListVM>>(allComments);

        }

    
        
    }
}