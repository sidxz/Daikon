
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.DocuStore
{
    public class ParsedDocVM : VMMeta
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string ExternalPath { get; set; } = string.Empty;
        public string DocHash { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        public object Title { get; set; } = string.Empty;
        public object Authors { get; set; } = string.Empty;
        public object ShortSummary { get; set; } = string.Empty;
        public object Notes { get; set; } = string.Empty;

        public HashSet<string> Tags { get; set; } = [];
        public HashSet<Guid> Mentions { get; set; } = [];
        public HashSet<string> ExtractedSMILES { get; set; } = [];
        public Dictionary<string, string> Molecules { get; set; } = [];


        public DVariable<DateTime>? PublicationDate { get; set; }
    }
}