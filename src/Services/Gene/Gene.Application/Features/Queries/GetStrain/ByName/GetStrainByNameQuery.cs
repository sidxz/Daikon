
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrain.ByName
{
    public class GetStrainByNameQuery : BaseQuery, IRequest<StrainVM>
    {
        public string Name { get; set; }
    }
}