using System.Text.Json.Serialization;
using CQRS.Core.Command;
using CQRS.Core.Converters;
using CQRS.Core.Domain;
using Daikon.Shared.Embedded.Screens;
using MediatR;

namespace Screen.Application.Features.Commands.NewHit
{
    public class NewHitCommand : BaseCommand, IRequest<Unit>
    {

        /* Identity */
        public Guid? HitId { get; set; }

        /* Library Information */
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Library { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? LibrarySource { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Method { get; set; }


        /* Assay */
        public string? AssayType { get; set; }



        /* Standard Measurements */
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? IC50 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? IC50Unit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? EC50 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? EC50Unit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Ki { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? KiUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Kd { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? KdUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MIC { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MICUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MICCondition { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MIC90 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MIC90Unit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? MIC90Condition { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? GI50 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? GI50Unit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? TGI { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? TGIUnit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? LD50 { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? LD50Unit { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PctInhibition { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PctInhibitionConcentration { get; set; }
        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? PctInhibitionConcentrationUnit { get; set; }

        /* Full Dose-Response Data */
        public List<DoseResponse>? DoseResponses { get; set; }




        /* Grouping and Notes */
        [JsonConverter(typeof(DVariableJsonConverter<int>))]
        public DVariable<int>? ClusterGroup { get; set; }

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? Notes { get; set; }



        /* Voting */

        [JsonConverter(typeof(DVariableJsonConverter<bool>))]
        public DVariable<bool>? IsVotingAllowed { get; set; }

        // userId, voting value
        public Dictionary<string, string>? Voters { get; set; }


        /* Molecule Metadata */

        [JsonConverter(typeof(DVariableJsonConverter<string>))]
        public DVariable<string>? RequestedSMILES { get; set; }
        public required string MoleculeName { get; set; }

        /* Disclosure Information */

        public string DisclosureScientist { get; set; } = string.Empty;
        public Guid DisclosureOrgId { get; set; } = Guid.Empty;


    }
}