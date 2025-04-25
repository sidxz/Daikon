
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.APIClients.MLogix
{
    public interface IMLogixAPI
    {
        public Task<MoleculeVM> GetMoleculeById(Guid Id);
        public Task<MoleculeVM> GetMoleculeBySmiles(string SMILES);
        public Task<List<MoleculeVM>> GetMoleculesByIds(List<Guid> Ids);
        public Task<MoleculeVM> RegisterMolecule(RegisterMoleculeDTO request);
        public Task<List<MoleculeVM>> RegisterBatch(List<RegisterMoleculeDTO> request);
    }
}