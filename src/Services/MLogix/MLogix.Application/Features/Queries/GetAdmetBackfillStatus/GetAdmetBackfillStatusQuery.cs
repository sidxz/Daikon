using CQRS.Core.Query;
using MediatR;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Queries.GetAdmetBackfillStatus
{
    public class GetAdmetBackfillStatusQuery : BaseQuery, IRequest<AdmetBackfillCountsVM>
    {
    }
}
