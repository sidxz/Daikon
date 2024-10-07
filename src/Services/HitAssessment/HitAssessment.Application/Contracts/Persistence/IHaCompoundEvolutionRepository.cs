
using HitAssessment.Domain.Entities;
using HitAssessment.Domain.EntityRevisions;

namespace HitAssessment.Application.Contracts.Persistence
{
    public interface IHaCompoundEvolutionRepository
    {
        Task CreateHaCompoundEvolution(HaCompoundEvolution haCompoundEvolution);
        Task<HaCompoundEvolution> ReadHaCompoundEvolutionById(Guid id);
        Task<List<HaCompoundEvolution>> GetHaCompoundEvolutionOfHa(Guid HaId);
        Task<Dictionary<Guid, List<HaCompoundEvolution>>> GetHaCompoundEvolutionsOfHAs(List<Guid> haIds);
        Task UpdateHaCompoundEvolution(HaCompoundEvolution haCompoundEvolution);
        Task DeleteHaCompoundEvolution(Guid id);
        Task<HaCompoundEvolutionRevision> GetHaCompoundEvolutionRevisions(Guid Id);
    }
}