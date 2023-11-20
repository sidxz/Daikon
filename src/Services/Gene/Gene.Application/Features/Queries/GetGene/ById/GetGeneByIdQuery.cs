using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ById
{
    public class GetGeneByIdQuery : IRequest<GeneVM>
    {
        public Guid Id { get; set; }
    }
}