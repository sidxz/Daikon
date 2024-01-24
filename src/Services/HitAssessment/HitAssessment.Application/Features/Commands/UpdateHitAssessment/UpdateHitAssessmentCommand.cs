
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
        
        public string? HaType { get; set; }
        public string? LegacyId { get; set; }
        

        public Dictionary<string, string> AssociatedHitIds { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime> HAStart { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime> HAPredictedStart { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> HADescription { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> HAStatus { get; set; }

        public bool IsHAComplete { get; set; }
        public DateTime? HAStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> PrimaryOrg { get; set; }
        
        public List<string> SupportingOrgs { get; set; }


    }
}