
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Application.Contracts.Infrastructure
{
    public interface IMolDbAPIService
    {
        public Task<CompoundDTO> RegisterCompound(string name, string initialCompoundStructure);
    }
}