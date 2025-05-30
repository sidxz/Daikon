
using CQRS.Core.Domain;
using CQRS.Core.Query;
using MediatR;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Queries.GetScreen.ByName
{
    public class GetScreenByNameQuery : BaseQuery, IRequest<ScreenVM>
    {
        public string Name { get; set; }
    }
}