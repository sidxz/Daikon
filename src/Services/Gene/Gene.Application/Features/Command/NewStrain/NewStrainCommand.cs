using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.NewStrain
{
    public class NewStrainCommand : BaseCommand, IRequest<Unit>
    {
        
        public string Name { get; set; }
        public string? Organism { get; set; }
       
    }
}