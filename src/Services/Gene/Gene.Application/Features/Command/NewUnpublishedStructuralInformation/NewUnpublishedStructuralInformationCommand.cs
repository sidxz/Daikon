
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.NewUnpublishedStructuralInformation
{
    public class NewUnpublishedStructuralInformationCommand : BaseCommand, IRequest<Unit>
    {
        
        public Guid GeneId { get; set; }
        public Guid? UnpublishedStructuralInformationId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public required DVariable<string> Organization { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Method { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Resolution { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? ResolutionUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Ligands { get; set; }
        
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Researcher { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Reference { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? URL { get; set; }
        


        
        
    }
}