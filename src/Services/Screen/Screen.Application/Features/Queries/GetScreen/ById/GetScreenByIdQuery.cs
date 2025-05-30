
using CQRS.Core.Domain;
using CQRS.Core.Query;
using MediatR;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Queries.GetScreen.ById
{
    public class GetScreenByIdQuery : BaseQuery, IRequest<ScreenVM>
    {
        public Guid Id { get; set; }
    }
}