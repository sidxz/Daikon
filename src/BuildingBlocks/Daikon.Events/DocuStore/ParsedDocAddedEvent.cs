using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.DocuStore
{
    public class ParsedDocAddedEvent : BaseEvent
    {
        public ParsedDocAddedEvent() : base(nameof(ParsedDocAddedEvent))
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

    }

}