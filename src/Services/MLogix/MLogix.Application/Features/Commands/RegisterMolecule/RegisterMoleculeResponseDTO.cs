
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeResponseDTO : MoleculeBase
    {
        public Guid RegistrationId { get; set; }
        public bool WasAlreadyRegistered { get; set; }
    }
}