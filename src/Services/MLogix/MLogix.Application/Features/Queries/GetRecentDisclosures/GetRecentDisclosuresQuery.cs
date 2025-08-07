using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Daikon.Shared.VM.MLogix;
using MediatR;
using MLogix.Domain.Entities;

namespace MLogix.Application.Features.Queries.GetRecentDisclosures
{
    public class GetRecentDisclosuresQuery : BaseQuery, IRequest<List<MoleculeVM>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}