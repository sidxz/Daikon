
using CQRS.Core.Domain;

namespace Screen.Application.Features.Queries.ViewModels
{
    public class ScreenVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> AssociatedTargets { get; set; }
        public string ScreenType { get; set; }
        public object Method { get; set; }
        public object Status { get; set; }
        public object LatestStatusChangeDate { get; set; }
        public object Notes { get; set; }
        public object PrimaryOrgId { get; set; }
        public object PrimaryOrgName { get; set; }

        public string Owner { get; set; }
        public object ExpectedCompleteDate { get; set; }
        public Dictionary<string, string> ParticipatingOrgs { get; set; }
        public List<ScreenRunVM> ScreenRuns { get; set; }
        //public List<HitCollectionVM> HitCollections { get; set; }
    }
}