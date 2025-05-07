
using CQRS.Core.Query;
using MediatR;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Queries.GetDefaults
{
    public class GetDefaultsQuery : BaseQuery, IRequest<TableDefaults>
    {
        public required string TableType { get; set; }
    }
}