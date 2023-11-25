using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain.Historical
{
    public class DVariableHistory<TDataType>
    {
        public DVariableHistory()
        {
            Versions = new List<VersionEntry<TDataType>>();
            CurrentVersion = 0;
            IsInitialVersion = true;
            CurrentModificationDate = DateTime.UtcNow;
        }

        /* Accepted Versions */
        public List<VersionEntry<TDataType>> Versions { get; set; }

        public DVariable<TDataType> CurrentValue { get; set; }
        public int CurrentVersion { get; set; }
        public string CurrentAuthor { get; set; }
        public bool IsInitialVersion { get; set; }
        public DateTime CurrentModificationDate { get; set; }

        public bool LegalFlag { get; set; }
        public string LegalFlagReason { get; set; }



    }
}