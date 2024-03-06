
using Screen.Application.Contracts.Infrastructure.DTOs;
using Screen.Application.DTOs.MLogixAPI;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Contracts.Infrastructure
{
    public interface IMLogixAPIService
    {
        public Task<RegisterMoleculeResponseDTO> RegisterCompound(RegisterMoleculeRequest registerMoleculeRequest);
        public Task<GetMoleculesResultDTO> GetMoleculeById(Guid id);
    }
}