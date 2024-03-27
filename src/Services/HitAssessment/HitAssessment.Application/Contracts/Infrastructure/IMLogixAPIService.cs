
using HitAssessment.Application.DTOs.MLogixAPI;

namespace HitAssessment.Application.Contracts.Infrastructure
{
    public interface IMLogixAPIService
    {
        public Task<RegisterMoleculeResponseDTO> RegisterCompound(RegisterMoleculeRequest registerMoleculeRequest);
        public Task<GetMoleculesResultDTO> GetMoleculeById(Guid id);
    }
}