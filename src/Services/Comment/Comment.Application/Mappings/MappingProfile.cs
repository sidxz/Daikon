using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Comment;
using Comment.Application.Features.Commands.NewComment;
using Comment.Application.Features.Commands.UpdateComment;
using Comment.Application.Features.Commands.DeleteComment;
using Comment.Application.Features.Commands.NewCommentReply;
using Comment.Application.Features.Commands.UpdateCommentReply;
using Comment.Application.Features.Commands.DeleteCommentReply;
using Comment.Application.Features.Queries.GetComment;
using Comment.Application.Features.Queries.GetCommentList;


namespace Comment.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /* Commands */
            CreateMap<CommentCreatedEvent, NewCommentCommand>().ReverseMap();
            CreateMap<CommentUpdatedEvent, UpdateCommentCommand>().ReverseMap();
            CreateMap<CommentDeletedEvent, DeleteCommentCommand>().ReverseMap();

            CreateMap<CommentReplyAddedEvent, NewCommentReplyCommand>().ReverseMap();
            CreateMap<CommentReplyUpdatedEvent, UpdateCommentReplyCommand>().ReverseMap();
            CreateMap<CommentReplyDeletedEvent, DeleteCommentReplyCommand>().ReverseMap();

            /* Events */
            CreateMap<Domain.Entities.Comment, CommentCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Comment, CommentUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Comment, CommentDeletedEvent>().ReverseMap();

            CreateMap<Domain.Entities.CommentReply, CommentReplyAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.CommentReply, CommentReplyUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.CommentReply, CommentReplyDeletedEvent>().ReverseMap();

            /* Queries */
            CreateMap<Domain.Entities.Comment, CommentListVM>().ReverseMap();

            CreateMap<Domain.Entities.Comment, CommentVM>()
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Comment, IValueProperty<string>, string>(src => src.Topic)))

            .ForMember(dest => dest.Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Comment, IValueProperty<string>, string>(src => src.Description)))

            .ReverseMap();

        }

    }
}