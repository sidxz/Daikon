
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.UpdateGene
{
    public class UpdateGeneCommand : BaseCommand, IRequest<Unit>
    {
        public string? UniProtKB { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? ProteinNameExpanded { get; set; }
        public string? AlphaFoldId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Product { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? FunctionalCategory { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Comments { get; set; }

        public Tuple<string, string, string>? Coordinates { get; set; } // (start, end, orientation)
        public List<Tuple<string, string>>? Orthologues { get; set; } // (type, description)

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? GeneSequence { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? ProteinSequence { get; set; }
        public string? GeneLength { get; set; }
        public string? ProteinLength { get; set; }
    }
}