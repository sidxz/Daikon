using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace DocuStore.Domain.Entities
{
    public class ParsedDoc : BaseEntity
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string ExternalPath { get; set; } = string.Empty;
        public string DocHash { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        public DVariable<string> Title { get; set; } = string.Empty;
        public DVariable<string> Authors { get; set; } = string.Empty;
        public DVariable<string> ShortSummary { get; set; } = string.Empty;

        public HashSet<string> Tags { get; set; } = [];
        public HashSet<string> ExtractedSMILES { get; set; } = [];
        public Dictionary<string, string> Molecules { get; set; } = [];


        public DVariable<DateTime>? PublicationDate { get; set; }

    }
}