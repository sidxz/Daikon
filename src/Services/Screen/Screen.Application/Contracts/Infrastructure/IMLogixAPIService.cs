
using Screen.Application.DTOs.MLogixAPI;

namespace Screen.Application.Contracts.Infrastructure
{
    public interface IMLogixAPIService
    {
        public Task<RegisterMoleculeResponseDTO> RegisterCompound(RegisterMoleculeRequest registerMoleculeRequest);
    }
}