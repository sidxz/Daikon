
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;


namespace Comment.Application.Features.Commands.UpdateCommentReply
{
    public class UpdateCommentReplyCommand : BaseCommand, IRequest<Unit>
    {
        public Guid CommentId { get; set; }
        public Guid ReplyId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Body { get; set; }
        public HashSet<string>? Tags { get; set; }
        public HashSet<Guid>? Mentions { get; set; }
        public HashSet<Guid>? Subscribers { get; set; }
        
    }
    
}