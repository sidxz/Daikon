
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using Gene.Domain.Entities;
using MediatR;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportOne
{
    public class BatchImportOneCommand : BaseCommand, IRequest<Unit>
    {
        
         public Guid? StrainId { get; set; }
        public string? StrainName { get; set; }
        public required string AccessionNumber { get; set; }
        public string? UniProtKB { get; set; }
        public string? Name { get; set; }
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



        // EXTRAS
        public List<GeneExpansionProp> ExpansionProps { get; set; }
        
        
    }
}