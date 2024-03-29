using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Screen.Application.Features.Commands.UpdateHitCollection
{
    public class UpdateHitCollectionCommand : BaseCommand, IRequest<Unit>
    {
        public required string HitCollectionType { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Owner { get; set; }
        
    }
}