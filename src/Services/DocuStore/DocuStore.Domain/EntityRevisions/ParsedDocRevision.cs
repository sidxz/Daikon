
using CQRS.Core.Domain.Historical;

namespace DocuStore.Domain.EntityRevisions
{
    public class ParsedDocRevision : BaseVersionEntity
    {
        public DVariableHistory<string> Title { get; set; }
        public DVariableHistory<string> Authors { get; set; }
        public DVariableHistory<string> ShortSummary { get; set; }
        public DVariableHistory<DateTime>? PublicationDate { get; set; }
    }
}