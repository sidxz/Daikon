
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.APIClients.MLogix
{
    public interface IMLogixAPI
    {
        public Task<MoleculeVM> GetMoleculeById(Guid Id);
        public Task<MoleculeVM> RegisterMolecule(RegisterMoleculeDTO request);
    }
}