
using CQRS.Core.Query;
using MediatR;

namespace MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId
{
    public class GetMoleculeByRegIdQuery : BaseQuery, IRequest<MoleculeVM>
    {
        public Guid RegistrationId { get; set; }
    }
}