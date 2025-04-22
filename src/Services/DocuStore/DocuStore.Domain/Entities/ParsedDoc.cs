
using CQRS.Core.Domain;
using Daikon.Shared.Embedded.DocuStore;

namespace DocuStore.Domain.Entities
{
    public class ParsedDoc : BaseEntity
    {
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


        public DVariable<DateTime> PublicationDate { get; set; } = new DVariable<DateTime> { Value = DateTime.UtcNow };

        public List<Reviews> Reviews { get; set; } = [];
        public List<Rating> Ratings { get; set; } = [];

        /*
         * Gets the average rating score, or null if there are no ratings.
         * This property is not persisted to MongoDB.
        */
        public double? AverageRating => Ratings.Count > 0 ? Ratings.Average(r => r.Score) : null;
        


    }
}