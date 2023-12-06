
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrain.ById
{
    public class GetStrainByIdQuery : BaseQuery, IRequest<StrainVM>
    {
        public Guid Id { get; set; }
    }
}