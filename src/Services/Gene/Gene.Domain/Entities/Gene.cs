using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gene.Domain.Entities
{
    public class Gene : BaseEntity
    {
        
        
        //public Guid StrainId { get; set; }
        public string StrainName { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public DVariable<string> Function { get; set; }
        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }

        
    }
}