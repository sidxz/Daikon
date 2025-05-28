
namespace Daikon.Shared.Embedded.DocuStore
{
    public class Reviews
    {
        public Guid Id { get; set; }
        public Guid Reviewer { get; set; }
        public string Review { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}