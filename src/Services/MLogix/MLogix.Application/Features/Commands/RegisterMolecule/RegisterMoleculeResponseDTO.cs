
using Daikon.Shared.VM.MLogix;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeResponseDTO : MoleculeVM
    {
        
        public bool WasAlreadyRegistered { get; set; }
    }
}