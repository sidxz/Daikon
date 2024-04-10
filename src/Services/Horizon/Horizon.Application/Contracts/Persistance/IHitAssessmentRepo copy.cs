
using Horizon.Domain.HitAssessment;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IHitAssessmentRepo
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task Create(HitAssessment hitAssessment);
        Task Update(HitAssessment hitAssessment);
        Task Delete(string hitAssessmentId);
    }
}