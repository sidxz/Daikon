
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.NewProteinActivityAssay
{
    public class NewProteinActivityAssayCommand : BaseCommand, IRequest<Unit>
    {
        
        public Guid GeneId { get; set; }
        public Guid? ProteinActivityAssayId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public required DVariable<string> Assay { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Method { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Throughput { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PMID { get; set; }
        
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Reference { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? URL { get; set; }
        
        
    }
}