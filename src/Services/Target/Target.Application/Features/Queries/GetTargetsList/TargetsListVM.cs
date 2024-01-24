
namespace Target.Application.Features.Queries.GetTargetsList
{
    public class TargetsListVM
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> AssociatedGenes { get; set; }
        public string TargetType { get; set; }
        public object Bucket { get; set; }
        public object ImpactScore { get; set; }
        public object ImpactComplete { get; set; }
        public object LikeScore { get; set; }
        public object LikeComplete { get; set; }
    }
}