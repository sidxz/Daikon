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
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
        public string Product { get; set; }
        public string FunctionalCategory { get; set; }
        //public List<Tuple<string, string>> ExternalIds { get; set; }
    }
}