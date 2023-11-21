using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Converters;
using CQRS.Core.Domain;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneVM
    {
        public Guid Id { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public object Function { get; set; }
        public object Product { get; set; }
        public object FunctionalCategory { get; set; }

    }
}