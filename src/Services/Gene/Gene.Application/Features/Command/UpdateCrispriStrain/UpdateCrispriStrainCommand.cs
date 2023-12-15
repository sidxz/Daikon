
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.UpdateCrispriStrain
{
    public class UpdateCrispriStrainCommand : BaseCommand, IRequest<Unit>
    {
        public Guid CrispriStrainId { get; set; }
        public Guid? GeneId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? CrispriStrainName { get; set; }
        
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }
        
    }
}