
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Target.Application.Features.Command.UpdateTarget
{
    public class UpdateTargetCommand : BaseCommand, IRequest<Unit>
    {
        public Guid StrainId { get; set; }
        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedGenes { get; set; }
        public string? TargetType { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Bucket { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? ImpactScore { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? ImpactComplete { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? LikeScore { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? LikeComplete { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? ScreeningScore { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? ScreeningComplete { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? StructureScore { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? StructureComplete { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? VulnerabilityRatio { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? VulnerabilityRank { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? HTSFeasibility { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? SBDFeasibility { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? Progressibility { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<double>))]
        public DVariable<double>? Safety { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Background { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Enablement { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Strategy { get; set; }
        
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Challenges { get; set; }

        public int Priority { get; set; } = default!;

    }
}