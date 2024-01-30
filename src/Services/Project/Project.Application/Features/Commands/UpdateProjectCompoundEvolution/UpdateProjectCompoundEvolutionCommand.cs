
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Project.Application.Features.Commands.UpdateProjectCompoundEvolution
{
    public class UpdateProjectCompoundEvolutionCommand : BaseCommand, IRequest<Unit>
    {
        public Guid CompoundEvolutionId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? CompoundStructureSMILES { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? EvolutionDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Stage { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MIC { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MICUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? IC50 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? IC50Unit { get; set; }

    }
}