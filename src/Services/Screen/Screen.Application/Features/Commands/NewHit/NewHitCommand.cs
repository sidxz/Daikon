using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Screen.Application.Features.Commands.NewHit
{
    public class NewHitCommand : BaseCommand, IRequest<Unit>
    {
        
        public Guid HitId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Library { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? LibrarySource { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Method { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MIC { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MICUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MICCondition { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? IC50 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? IC50Unit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<int>))]
        public DVariable<int>? ClusterGroup { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }

        /* Voting */

        [JsonConverter(typeof(DVariableJsonConverter<bool>))]
        public DVariable<bool>? IsVotingAllowed { get; set; }

        // userId, voting value
        public Dictionary<string, string>? Voters { get; set; }

        /* Compound */

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? RequestedSMILES { get; set; }
        public string? MoleculeName { get; set; }
        
    }
}