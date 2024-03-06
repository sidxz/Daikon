
using CQRS.Core.Query;
using MediatR;

namespace MLogix.Application.Features.Queries.GetMolecule.ById
{
    public class GetMoleculeByIdQuery : BaseQuery, IRequest<MoleculeVM>
    {
        public Guid Id { get; set; }
    }
}