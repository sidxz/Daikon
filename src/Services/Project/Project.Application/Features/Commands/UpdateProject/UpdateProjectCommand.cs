
using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Project.Application.Features.Commands.UpdateProject
{
    public class UpdateProjectCommand : BaseCommand, IRequest<Unit>
    {

        public string? Alias { get; set; }
        public string? ProjectType { get; set; }
        public string? LegacyId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Description { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Status { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Stage { get; set; }

        public bool? IsProjectComplete { get; set; }
        public bool? IsProjectRemoved { get; set; }

        /* Orgs */
        [JsonConverter(typeof(DVariableJsonConverter<Guid>))]
        public DVariable<Guid>? PrimaryOrgId { get; set; }
        public List<Guid>? ParticipatingOrgs { get; set; }


        /* Priority */
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Priority { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Probability { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PriorityNote { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? ProbabilityNote { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? PPLastStatusDate { get; set; }

        /* Project Manager */

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PmPriority { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PmProbability { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PmPriorityNote { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PmProbabilityNote { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? PmPPLastStatusDate { get; set; }



        /* Dates */
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? H2LPredictedStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? H2LStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? LOPredictedStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? LOStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? SPPredictedStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? SPStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? INDPredictedStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? INDStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? P1PredictedStart { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? P1Start { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? CompletionDate { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? ProjectRemovedDate { get; set; }

        public DateTime? ProjectStatusDate { get; set; }

        public DateTime? TerminationDate { get; set; }


    }
}