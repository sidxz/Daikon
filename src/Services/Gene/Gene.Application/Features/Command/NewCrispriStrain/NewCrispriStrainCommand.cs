
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.NewCrispriStrain
{
    public class NewCrispriStrainCommand : BaseCommand, IRequest<Unit>
    {
        
        public Guid GeneId { get; set; }
        public Guid? CrispriStrainId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public required DVariable<string> CrispriStrainName { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }
        
        
    }
}