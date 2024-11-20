using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace EventHistory.Application.Features.Queries.GetEventHistory
{
    public class GetEventHistoryQuery : BaseQuery, IRequest<List<EventHistoryVM>>
    {
        public List<Guid>? AggregateIds { get; set; }
        public List<string>? AggregateTypes { get; set; }
        public List<string>? EventTypes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Limit { get; set; } = 100;
        public bool RefreshCache { get; set; } = false;
    }
}