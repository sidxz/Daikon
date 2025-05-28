
using CQRS.Core.Query;
using MediatR;

namespace UserPreferences.Application.Features.Queries.ResolveTableConfig
{
    public class ResolveTableConfigQuery : BaseQuery, IRequest<ResolveTableConfigVM>
    {
        public required string TableType { get; set; }
        public required Guid TableInstanceId { get; set; }
    }
}