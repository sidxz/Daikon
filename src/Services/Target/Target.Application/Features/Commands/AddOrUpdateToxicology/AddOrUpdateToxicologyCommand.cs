using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using MediatR;

namespace Target.Application.Features.Commands.AddOrUpdateToxicology
{
    public class AddOrUpdateToxicologyCommand : BaseCommand, IRequest<Unit>
    {
        public Guid TargetId { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public required DVariable<string> Topic { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Impact { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<bool>))]
        public DVariable<bool> ImpactPriority { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Likelihood { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<bool>))]
        public DVariable<bool> LikelihoodPriority { get; set; }


        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string> Note { get; set; }

        // default constructor
        public AddOrUpdateToxicologyCommand()
        {

            Impact = new DVariable<string>() { Value = default! };
            ImpactPriority = new DVariable<bool>() { Value = default! };
            Likelihood = new DVariable<string>() { Value = default! };
            LikelihoodPriority = new DVariable<bool>() { Value = default! };
            Note = new DVariable<string>() { Value = default! };
        }
    }
}