
using Daikon.Shared.VM.MLogix;

namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    public class RegisterMoleculeResponseDTO : MoleculeVM
    {
        
        public bool WasAlreadyRegistered { get; set; }
        public string PreviewMessage { get; set; } = string.Empty;
        public string PreviewStatus { get; set; } = string.Empty;
    }
}