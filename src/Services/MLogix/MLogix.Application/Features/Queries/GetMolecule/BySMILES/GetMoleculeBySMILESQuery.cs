
using CQRS.Core.Query;
using Daikon.Shared.VM.MLogix;
using MediatR;

namespace MLogix.Application.Features.Queries.GetMolecule.BySMILES
{
    public class GetMoleculeBySMILESQuery : BaseQuery, IRequest<MoleculeVM>
    {
        public string SMILES { get; set; }
    }
}