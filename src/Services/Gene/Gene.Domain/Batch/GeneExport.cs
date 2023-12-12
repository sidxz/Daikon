
using Gene.Domain.Entities;

namespace Gene.Domain.Batch
{
    public class GeneExport : Entities.Gene
    {
        public List<Essentiality> Essentialities { get; set; }
    }
}