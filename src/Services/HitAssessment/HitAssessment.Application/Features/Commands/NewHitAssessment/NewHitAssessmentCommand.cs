
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace HitAssessment.Application.Features.Commands.NewHitAssessment
{
    public class NewHitAssessmentCommand : BaseCommand, IRequest<Unit>
    {
        public Guid? StrainId { get; set; }
        public string Name { get; set; }
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        public Guid? HitId { get; set; }
        public Guid? CompoundId { get; set; }
        public Dictionary<string, string>? AssociatedHitIds { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? HaStartDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? HaPredictedStartDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Description { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<Guid>))]
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }

    }
}