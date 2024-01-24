
namespace HitAssessment.Application.Contracts.Persistence
{
    public interface IHitAssessmentRepository
    {
        Task CreateHa(Domain.Entities.HitAssessment hitAssessment);
        Task<Domain.Entities.HitAssessment> ReadHaByName(string name);
        Task<Domain.Entities.HitAssessment> ReadHaById(Guid id);

        Task<List<Domain.Entities.HitAssessment>> GetHaList();
        Task<List<Domain.Entities.HitAssessment>> GetHaListByStrainId(Guid strainId);
        Task UpdateHa(Domain.Entities.HitAssessment ha);
        Task DeleteHa(Guid id);
        Task<Domain.EntityRevisions.HitAssessmentRevision> GetHaRevisions(Guid Id);
    }
}