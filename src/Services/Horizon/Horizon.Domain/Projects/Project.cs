

namespace Horizon.Domain.Projects
{
    public class Project : GraphEntity
    {   
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Stage { get; set; }
        public bool IsProjectComplete { get; set; }
        public bool IsProjectRemoved { get; set; }
        public string HitAssessmentId { get; set; }
        public string PrimaryMoleculeId { get; set; }
        public string HitMoleculeId { get; set; }
        public string OrgId { get; set; }
    }
}