
using CQRS.Core.Query;

using MediatR;

namespace Project.Application.Features.Queries.GetProjectList
{
    public class GetProjectListQuery : BaseQuery, IRequest<List<ProjectListVM>>
    {
    }
}