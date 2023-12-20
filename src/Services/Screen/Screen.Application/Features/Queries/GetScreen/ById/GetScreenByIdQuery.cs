
using CQRS.Core.Domain;
using CQRS.Core.Query;
using MediatR;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetScreen.ById
{
    public class GetScreenByIdQuery : BaseQuery, IRequest<ScreenVM>
    {
        public Guid Id { get; set; }
    }
}