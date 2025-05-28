
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using Daikon.Shared.Embedded.DocuStore;
using DocuStore.Domain.Entities;
using MediatR;

namespace DocuStore.Application.Features.Commands.UpdateParsedDoc
{
    public class UpdateParsedDocCommand : BaseCommand, IRequest<ParsedDoc>
    {
        public string Name { get; set; } = string.Empty;
        public required string FilePath { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string ExternalPath { get; set; } = string.Empty;
        public string DocHash { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Title { get; set; } = new DVariable<string>();

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Authors { get; set; } = new DVariable<string>();

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> ShortSummary { get; set; } = new DVariable<string>();
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Notes { get; set; } = new DVariable<string>();

        public HashSet<string> Tags { get; set; } = [];
        public HashSet<Guid> Mentions { get; set; } = [];
        public HashSet<string> ExtractedSMILES { get; set; } = [];
        public Dictionary<string, string> Molecules { get; set; } = [];

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? PublicationDate { get; set; }
        public List<Reviews> Reviews { get; set; } = [];
        public List<Rating> Ratings { get; set; } = [];

    }
}