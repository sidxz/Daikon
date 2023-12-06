using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Converters;
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetStrain
{
    public class StrainVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Organism { get; set; }
        
    }
}