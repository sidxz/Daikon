using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public class DVariable<TDataType>
    {
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TDataType Value { get; set; }

        // /* Version Meta Data */
        // public string ChangeType { get; set; } // Addition, Modification, Deletion
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Author { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? Date { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Comment { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Provenance { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Source { get; set; }

        // /* ML Generated Properties */
        // public bool IsMLGenerated { get; set; }
        // public string MLGeneratedBy { get; set; }
        // public DateTime MLGeneratedDate { get; set; }
        // public string MLGeneratedComment { get; set; }
        // public float MLGeneratedConfidence { get; set; }


        // /* Approval Properties */
        // public string ApprovalStatus { get; set; }
        // public bool IsVerified { get; set; }
        // public string VerifiedBy { get; set; }
        // public string VerifiedComment { get; set; }
        // public DateTime VerifiedDate { get; set; }


        // /* Data Quality Properties */
        // public string DataQualityIndicator { get; set; }


        // public bool IsArchived { get; set; }
        
    }
}