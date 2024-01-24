
namespace Project.Application.Features.Queries.GetProjectList
{
    public class ProjectListVM
    {
        public Guid Id { get; set; }
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }
        public object ProjectStart { get; set; }
        public object ProjectPredictedStart { get; set; }
        public object ProjectStatus { get; set; }

        public bool IsProjectComplete { get; set; }
        public object PrimaryOrg { get; set; }
    }
}