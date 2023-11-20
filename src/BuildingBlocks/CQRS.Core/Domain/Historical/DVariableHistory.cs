using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain.Historical
{
    public class DVariableHistory<TDataType>
    {
        /* Accepted Versions */
        public List<VersionEntry<TDataType>> Versions { get; set; }
       
        public TDataType CurrentValue { get; set; }
        public int CurrentVersion { get; set; }
        public string CurrentAuthor { get; set; }
        public bool IsInitialVersion { get; set; }
        public DateTime CurrentModificationDate { get; set; }

        public bool LegalFlag { get; set; }
        public string LegalFlagReason { get; set; }





        /* Proposed Versions */
        public List<VersionEntry<TDataType>> ProposedVersions { get; set; }
        public bool IsVersionProposed { get; set; }

        /* Rejected Versions */
        public List<VersionEntry<TDataType>> RejectedVersions { get; set; }

    }
}