using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Comment.Application.Features.Commands.NewComment
{
    public class NewCommentCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ResourceId { get; set; }
        public string? Reference { get; set; }
        public string? Section { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Topic { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Description { get; set; }
        public string? PostedBy { get; set; }

    }
}