
namespace Project.Application.DTOs.MLogixAPI
{
    public class RegisterMoleculeRequest
    {
        public string? Name { get; set; }
        public string RequestedSMILES { get; set; }
        public List<string>? Synonyms   { get; set; }
        public Dictionary<string, string>? Ids { get; set; }
    }
}