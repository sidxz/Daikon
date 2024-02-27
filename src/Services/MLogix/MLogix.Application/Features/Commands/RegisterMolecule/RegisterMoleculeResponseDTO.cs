
using System.Text.Json.Serialization;
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeResponseDTO
    {
        public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }
        public string? Name { get; set; }
        public CompoundDTO? Calculated { get; set; }
    }
}