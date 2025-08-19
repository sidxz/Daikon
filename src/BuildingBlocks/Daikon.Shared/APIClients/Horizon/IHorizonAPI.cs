

using Daikon.Shared.VM.Horizon;

namespace Daikon.Shared.APIClients.Horizon
{
    public interface IHorizonAPI
    {
        // public Task<HitAssessmentVM> GetById(Guid id, bool forceRefresh = false);
        // public Task<List<HitAssessmentVM>> GetList(bool forceRefresh = false);
        public Task<CompoundRelationsMultipleVM> GetCompoundRelationsMultiple(List<Guid> ids, bool forceRefresh = false);
    }
}