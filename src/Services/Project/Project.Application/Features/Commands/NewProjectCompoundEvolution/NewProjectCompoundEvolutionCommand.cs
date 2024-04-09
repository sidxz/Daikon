
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;
using Project.Application.Features.Commands.NewHaCompoundEvolution;

namespace Project.Application.Features.Commands.NewProjectCompoundEvolution
{
    public class NewProjectCompoundEvolutionCommand : BaseCommand, IRequest<NewProjectCompoundEvolutionResDTO>
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

    }
}