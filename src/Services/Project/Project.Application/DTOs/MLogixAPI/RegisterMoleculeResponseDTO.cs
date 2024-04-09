using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Application.DTOs.MLogixAPI
{
    public class RegisterMoleculeResponseDTO
    {
        public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }
        public string? Name { get; set; }
        public List<string>? Synonyms { get; set; }
        public Dictionary<string, string>? Ids { get; set; }
        public bool WasAlreadyRegistered { get; set; }
        public float? Similarity { get; set; }
        public string Smiles { get; set; }
        public string? SmilesCanonical { get; set; }
        public float? MolecularWeight { get; set; }
        public float? TPSA { get; set; }
    }
}