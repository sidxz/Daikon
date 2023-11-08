using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.NewGene
{
    public class NewGeneCommand : BaseCommand, IRequest<int>
    {
        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
        public string Product { get; set; }
        public string FunctionalCategory { get; set; }
        public List<Tuple<string, string>> ExternalIds { get; set; }
        
    }
}