
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace HitAssessment.Application.Features.Commands.UpdateHitAssessment
{
    public class UpdateHitAssessmentCommand : BaseCommand, IRequest<Unit>
    {
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Description { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Status { get; set; }

        public bool? IsHASuccess { get; set; }
        public bool? IsHAComplete { get; set; }
        public bool? IsHAPromoted { get; set; }

        // Primary HA Compound
        public Guid? HitId { get; set; }
        public Guid CompoundId { get; set; }
        public string? CompoundSMILES { get; set; }
        public string? CompoundMIC { get; set; }
        public string? CompoundIC50 { get; set; }

        // Associated HA Compounds
        public Dictionary<string, string>? AssociatedHitIds { get; set; }

        // Orgs
        [JsonConverter(typeof(DVariableJsonConverter<Guid>))]
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }


        // Dates
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
        public DVariable<DateTime>? TerminationDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? CompletionDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? EOLDate { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? H2LPredictedStartDate { get; set; }

    }
}