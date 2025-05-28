
using CQRS.Core.Query;
using MediatR;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Queries.GetScreensList
{
    public class GetScreensListQuery : BaseQuery, IRequest<List<ScreensListVM>>
    {

    }
}