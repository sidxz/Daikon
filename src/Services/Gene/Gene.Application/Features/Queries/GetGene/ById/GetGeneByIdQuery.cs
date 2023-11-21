using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ById
{
    public class GetGeneByIdQuery : BaseQuery, IRequest<GeneVM>
    {
        public Guid Id { get; set; }
    }
}