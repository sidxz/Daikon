using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ByAccession
{
    public class GetGeneByAccessionQuery : BaseQuery, IRequest<GeneVM>
    {
        public string AccessionNumber { get; set; }
    }
}