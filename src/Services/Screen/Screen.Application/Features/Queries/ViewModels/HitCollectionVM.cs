
namespace Screen.Application.Features.Queries.ViewModels
{
    public class HitCollectionVM
    {
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }
        public string Name { get; set; }
        public string HitCollectionType { get; set; }
        public object Notes { get; set; }
        public object Owner { get; set; }
        public List<HitVM> Hits { get; set; }
    }
}