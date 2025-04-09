
using System.Text.Json.Serialization;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using MongoDB.Bson.Serialization.Attributes;

namespace Daikon.Events.HitAssessment
{
    [BsonDiscriminator(nameof(HaCreatedEvent))]
    public class HaCreatedEvent : BaseEvent
    {
        public HaCreatedEvent() : base(nameof(HaCreatedEvent))
        {

        }

        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Description { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Status { get; set; }


        public bool IsHAComplete { get; set; }
        public bool IsHASuccess { get; set; }
        public bool IsHAPromoted { get; set; }

        /* Associated Hit */
        public Guid HitId { get; set; }
        public Guid HitCollectionId { get; set; }
        public Guid CompoundId { get; set; }
        public Dictionary<string, string> AssociatedHitIds { get; set; }




        /* Orgs */
        [JsonConverter(typeof(DVariableJsonConverter<Guid>))]
        public DVariable<Guid>? PrimaryOrgId { get; set; }

        public List<Guid>? ParticipatingOrgs { get; set; }


        /* Dates */
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? HaPredictedStartDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? HaStartDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusLastModifiedDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusReadyForHADate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusActiveDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusIncorrectMzDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusKnownLiabilityDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusCompleteFailedDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusCompleteSuccessDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? RemovalDate { get; set; }

        
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? CompletionDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? StatusPausedDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? EOLDate { get; set; }

        
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? H2LPredictedStartDate { get; set; }

    }
}