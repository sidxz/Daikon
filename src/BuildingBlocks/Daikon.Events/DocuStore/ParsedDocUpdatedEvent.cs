
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.Shared.Embedded.DocuStore;
namespace Daikon.Events.DocuStore
{
    public class ParsedDocUpdatedEvent : BaseEvent
    {
        public ParsedDocUpdatedEvent() : base(nameof(ParsedDocUpdatedEvent))
        {
            
        }

        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string ExternalPath { get; set; } = string.Empty;
        public string DocHash { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        public DVariable<string> Title { get; set; } = new();
        public DVariable<string> Authors { get; set; } = new();
        public DVariable<string> ShortSummary { get; set; } = new();
        public DVariable<string> Notes { get; set; } = new();

        public HashSet<string> Tags { get; set; } = [];
        public HashSet<Guid> Mentions { get; set; } = [];
        public HashSet<string> ExtractedSMILES { get; set; } = [];
        public Dictionary<string, string> Molecules { get; set; } = [];


        public DVariable<DateTime>? PublicationDate { get; set; }
        public List<Reviews> Reviews { get; set; } = [];
        public List<Rating> Ratings { get; set; } = [];

    }
}