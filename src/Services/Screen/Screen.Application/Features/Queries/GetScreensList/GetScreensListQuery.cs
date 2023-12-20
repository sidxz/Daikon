
using CQRS.Core.Query;
using MediatR;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetScreensList
{
    public class GetScreensListQuery : BaseQuery, IRequest<List<ScreensListVM>>
    {

    }
}