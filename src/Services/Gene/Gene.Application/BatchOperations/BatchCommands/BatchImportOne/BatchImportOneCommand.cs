
using CQRS.Core.Command;
using CQRS.Core.Domain;
using Gene.Domain.Entities;
using MediatR;

namespace Gene.Application.BatchOperations.BatchCommands.BatchImportOne
{
    public class BatchImportOneCommand : BaseCommand, IRequest<Unit>
    {
        
        public string AccessionNumber { get; set; }
        public Guid? StrainId { get; set; }
        public string? StrainName { get; set; }
        public string? Name { get; set; }
        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }


        public List<Essentiality> Essentialities { get; set; }
        
        
    }
}