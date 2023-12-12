
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using Gene.Domain.Entities;
using MediatR;

namespace Gene.Application.Features.Command.BatchImportOne
{
    public class BatchImportOneCommand : BaseCommand, IRequest<Unit>
    {
        
        public string AccessionNumber { get; set; }
        public Guid? StrainId { get; set; }
        public string? StrainName { get; set; }
        public string? Name { get; set; }
        public DVariable<string> Function { get; set; }
        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }


        public List<Essentiality> Essentialities { get; set; }
        
        
    }
}