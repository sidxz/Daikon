
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.NewEssentiality
{
    public class NewEssentialityCommand : BaseCommand, IRequest<Unit>
    {
        
        public Guid EssentialityId { get; set; }
        public required DVariable<string> Classification { get; set; }
        public DVariable<string>? Condition { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Reference { get; set; }
        public DVariable<string>? Note { get; set; }
        
        
    }
}