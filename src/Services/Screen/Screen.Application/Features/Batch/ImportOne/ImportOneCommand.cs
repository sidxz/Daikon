using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;
using Screen.Domain.Entities;

namespace Screen.Application.Features.Batch.ImportOne
{
    public class ImportOneCommand : BaseCommand, IRequest<Unit>
    {
        public Guid StrainId { get; set; }
        public required string Name { get; set; }
        public Dictionary<string, string>? AssociatedTargets { get; set; }
        public string? ScreenType { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Method { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Status { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<Guid>))]
        public DVariable<Guid>? PrimaryOrgId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PrimaryOrgName { get; set; }

        public Dictionary<string, string>? ParticipatingOrgs { get; set; }
        public string? Owner { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<DateTime>))]
        public DVariable<DateTime>? ExpectedCompleteDate { get; set; }

        public List<ScreenRunDTO>? ScreenRuns { get; set; }
        public List<HitDTO>? Hits { get; set; }
    }
}