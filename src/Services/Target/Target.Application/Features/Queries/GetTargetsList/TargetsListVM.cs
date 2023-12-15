namespace Target.Application.Features.Queries.GetTargetsList
{
    public class TargetsListVM
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> AssociatedGenes { get; set; }
        public string TargetType { get; set; }
        public string Bucket { get; set; }
        public string ImpactScore { get; set; }
        public string LikeScore { get; set; }
        
        



    }
}