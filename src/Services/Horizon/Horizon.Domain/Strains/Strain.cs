
namespace Horizon.Domain.Strains
{
    public class Strain : GraphEntity
    {
        public string StrainId { get; set; }
        public string Name { get; set; }
        public string? Organism { get; set; }
        
    }
}