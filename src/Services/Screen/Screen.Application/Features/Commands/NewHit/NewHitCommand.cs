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
        public Guid HitCollectionId { get; set; }

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

        [JsonConverter(typeof(DVariableJsonConverter<int>))]
        public DVariable<int>? Positive { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<int>))]
        public DVariable<int>? Neutral { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<int>))]
        public DVariable<int>? Negative { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<bool>))]
        public DVariable<bool>? IsVotingAllowed { get; set; }

        /* Compound */

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? InitialCompoundStructure { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? CompoundRegistrationStatus { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<Guid>))]
        public DVariable<Guid>? CompoundId { get; set; }
        
    }
}