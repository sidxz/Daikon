using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CQRS.Core.Domain.Historical
{
    public class VersionEntry<TDataType>
    {
        public VersionEntry()
        {
            VersionDetails = new DVariable<TDataType>();
        }
        
        public int VersionNumber { get; set; }
        public DVariable<TDataType> VersionDetails { get; set; }
    }
}