using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Gene.Application.Features.Command.UpdateStrain
{
    public class UpdateStrainCommand : BaseCommand, IRequest<Unit>
    {
        public string Name { get; set; }
        public string? Organism { get; set; }
        
    }
}