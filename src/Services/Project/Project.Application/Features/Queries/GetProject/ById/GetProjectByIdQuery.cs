
using CQRS.Core.Query;
using MediatR;

namespace Project.Application.Features.Queries.GetProject.ById
{
    public class GetProjectByIdQuery : BaseQuery, IRequest<ProjectVM>
    {
        public Guid Id { get; set; }
    }
}