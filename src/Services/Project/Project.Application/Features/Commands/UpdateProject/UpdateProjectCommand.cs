
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Project.Application.Features.Commands.UpdateProject
{
    public class UpdateProjectCommand : BaseCommand, IRequest<Unit>
    {
        public Guid? StrainId { get; set; }
        
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }
        

        public Dictionary<string, string> AssociatedHitIds { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime> ProjectStart { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime> ProjectPredictedStart { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> ProjectDescription { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> ProjectStatus { get; set; }

        public bool IsProjectComplete { get; set; }
        public DateTime? ProjectStatusDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? EOLDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> PrimaryOrg { get; set; }
        
        public List<string> SupportingOrgs { get; set; }


    }
}