
using Horizon.Domain.HitAssessment;

namespace Horizon.Application.Contracts.Persistence
{
    public interface IHitAssessmentRepo
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task Create(HitAssessment hitAssessment);
        Task Update(HitAssessment hitAssessment);
        Task Rename(HitAssessment hitAssessment);
        Task Delete(string hitAssessmentId);

        Task AddHaCEvo(HACompoundEvolution haCompoundEvolution);
        Task UpdateHaCEvo(HACompoundEvolution haCompoundEvolution);
        Task DeleteHaCEvo(string compoundEvoId);
    }
}