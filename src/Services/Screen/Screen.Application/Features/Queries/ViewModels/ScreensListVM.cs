
namespace Screen.Application.Features.Queries.ViewModels
{
    public class ScreensListVM
    {
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> AssociatedTargets { get; set; }
        public string ScreenType { get; set; }
        public object Method { get; set; }
        public object Status { get; set; }
        public DateTime LastStatusChangedDate { get; set; }
        public object PrimaryOrgId { get; set; }
        public object PrimaryOrgName { get; set; }
        public string Owner { get; set; }
        public object ExpectedCompleteDate { get; set; }
        public Dictionary<string, string> ParticipatingOrgs { get; set; }
    }
}