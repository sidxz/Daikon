
namespace HitAssessment.Application.Contracts.Persistence
{
    public interface IHaCompoundEvolutionRepository
    {
        Task CreateHaCompoundEvolution(Domain.Entities.HaCompoundEvolution haCompoundEvolution);
        Task<Domain.Entities.HaCompoundEvolution> ReadHaCompoundEvolutionById(Guid id);
        Task<List<Domain.Entities.HaCompoundEvolution>> GetHaCompoundEvolutionOfHa(Guid HaId);
        Task UpdateHaCompoundEvolution(Domain.Entities.HaCompoundEvolution haCompoundEvolution);
        Task DeleteHaCompoundEvolution(Guid id);
        Task<Domain.EntityRevisions.HaCompoundEvolutionRevision> GetHaCompoundEvolutionRevisions(Guid Id);
    }
}