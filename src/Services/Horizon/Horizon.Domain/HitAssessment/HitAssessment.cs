using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Domain.HitAssessment
{
    public class HitAssessment : GraphEntity
    {   
        public string HitAssessmentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsHAComplete { get; set; }
        public bool IsHASuccess { get; set; }
        public string HitCollectionId { get; set; }
        public string PrimaryMoleculeId { get; set; }
        public List<string> AssociatedMoleculeIds { get; set; }

        public string OrgId { get; set; }
    }
}