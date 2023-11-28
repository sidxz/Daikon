using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetGeneVersions.ById
{
    public class GetGeneVersionsQuery : BaseQuery, IRequest<GeneVersionsVM>
    {
        public Guid Id { get; set; }
    }
}