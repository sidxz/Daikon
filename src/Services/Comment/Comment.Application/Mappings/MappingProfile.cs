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


            /* ====== Comment Core ====== */

            // -- Commands --
            CreateMap<Domain.Entities.Comment, Domain.Entities.Comment>();

            CreateMap<CommentCreatedEvent, NewCommentCommand>().ReverseMap();
            CreateMap<CommentUpdatedEvent, UpdateCommentCommand>().ReverseMap();
            CreateMap<CommentDeletedEvent, DeleteCommentCommand>().ReverseMap();

            CreateMap<CommentCreatedEvent, Domain.Entities.Comment>().ReverseMap();
            CreateMap<CommentUpdatedEvent, Domain.Entities.Comment>().ReverseMap();
            CreateMap<CommentDeletedEvent, Domain.Entities.Comment>().ReverseMap();

            // -- Queries --

            CreateMap<Domain.Entities.Comment, CommentListVM>().ReverseMap();
            CreateMap<Domain.Entities.Comment, CommentVM>()
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Comment, IValueProperty<string>, string>(src => src.Topic)))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Comment, IValueProperty<string>, string>(src => src.Description)))
            .ReverseMap();


            /* ====== Reply ====== */
            // -- Commands --
            CreateMap<Domain.Entities.CommentReply, Domain.Entities.CommentReply>();

            CreateMap<CommentReplyAddedEvent, NewCommentReplyCommand>().ReverseMap();
            CreateMap<CommentReplyUpdatedEvent, UpdateCommentReplyCommand>().ReverseMap();
            CreateMap<CommentReplyDeletedEvent, DeleteCommentReplyCommand>().ReverseMap();

            CreateMap<CommentReplyAddedEvent, Domain.Entities.CommentReply>().ReverseMap();
            CreateMap<CommentReplyUpdatedEvent, Domain.Entities.CommentReply>().ReverseMap();
            CreateMap<CommentReplyDeletedEvent, Domain.Entities.CommentReply>().ReverseMap();

            // -- Queries --
            CreateMap<Domain.Entities.CommentReply, CommentReplyVM>()
            .ForMember(dest => dest.Body, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.CommentReply, IValueProperty<string>, string>(src => src.Body)))
            .ReverseMap();

        }

    }
}