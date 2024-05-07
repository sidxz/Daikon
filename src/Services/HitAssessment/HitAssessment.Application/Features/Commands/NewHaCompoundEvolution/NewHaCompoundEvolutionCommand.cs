
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionCommand : BaseCommand, IRequest<NewHaCompoundEvolutionResDTO>
    {

        public Guid CompoundEvolutionId { get; set; }

        public Guid? MoleculeId { get; set; }
        public string MoleculeName { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? RequestedSMILES { get; set; }

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

        public bool? ImportMode { get; set; }

    }
}