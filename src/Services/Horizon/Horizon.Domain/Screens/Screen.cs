
namespace Horizon.Domain.Screens
{
    public class Screen
    {
        public Guid? StrainId { get; set; }
        public required string Name { get; set; }
        public List<Guid> AssociatedTargets { get; set; }
        public string ScreenType { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string PrimaryOrgName { get; set; }
    }
}