using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Comment.Application.Features.Commands.NewComment
{
    public class NewCommentCommand : BaseCommand, IRequest<Unit>
    {

        public Guid? ResourceId { get; set; }
        

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Topic { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Description { get; set; }

        public HashSet<string>? Tags { get; set; }
        public HashSet<Guid>? Mentions { get; set; }
        public HashSet<Guid>? Subscribers { get; set; }
        public bool? IsCommentLocked { get; set; }

    }
}