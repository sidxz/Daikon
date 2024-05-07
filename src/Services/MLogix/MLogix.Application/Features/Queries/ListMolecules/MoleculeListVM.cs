
namespace MLogix.Application.Features.Queries.ListMolecules
{
    public class MoleculeListVM
    {
        public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }
        public string Name { get; set; }
        public List<string> Synonyms { get; set; }
        public Dictionary<string, string> Ids { get; set; }
        public float Similarity { get; set; }
        public string Smiles { get; set; }
        public string? SmilesCanonical { get; set; }
        public float? MolecularWeight { get; set; }
        public float? TPSA { get; set; }
    }
}