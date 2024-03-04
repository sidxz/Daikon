
using CQRS.Core.Domain;

namespace Screen.Application.Features.Queries.ViewModels
{
    public class ScreensListVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> AssociatedTargets { get; set; }
        public string AssociatedTargetsFlattened { get; set; }
        public string ScreenType { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public DateTime LatestStatusChangeDate { get; set; }
        public Guid PrimaryOrgId { get; set; }
        public string PrimaryOrgName { get; set; }
        public string Owner { get; set; }
        public DateTime ExpectedCompleteDate { get; set; }
        public Dictionary<string, string> ParticipatingOrgs { get; set; }
    }
}