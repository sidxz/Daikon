
using Gene.Domain.Entities;

namespace Gene.Application.BatchOperations.BatchQueries.DTOs
{
    public class GeneExportDto : Domain.Entities.Gene
    {
        public List<Essentiality> Essentialities { get; set; }
    }
}