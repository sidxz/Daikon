
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;


namespace Comment.Application.Features.Commands.UpdateCommentReply
{
    public class UpdateCommentReplyCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ReplyId { get; set; }
        public Guid ResourceId { get; set; }
        
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Body { get; set; }
        public string? PostedBy { get; set; }
        
    }
    
}