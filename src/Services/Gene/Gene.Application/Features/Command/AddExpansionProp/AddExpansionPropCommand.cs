
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.AddExpansionProp
{
    public class AddExpansionPropCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ExpansionPropId { get; set; }
        public required string ExpansionType { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public required DVariable<string> ExpansionValue { get; set; }
    }
}