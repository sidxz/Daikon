
using Daikon.Shared.VM.HitAssessment;

namespace Daikon.Shared.APIClients.HitAssessment
{
    public interface IHitAssessmentAPI
    {
        public Task<HitAssessmentVM> GetById(Guid id, bool forceRefresh = false);
        public Task<List<HitAssessmentVM>> GetList(bool forceRefresh = false);
    }
}