
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using MediatR;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using MLogix.Application.Features.Commands.UpdateMolecule;
using MLogix.Application.Features.Queries.FindSimilarMolecules;
using MLogix.Application.Features.Queries.FindSubstructures;
using MLogix.Application.Features.Queries.GetMolecule.ByName;

namespace MLogix.Application.Contracts.Infrastructure.DaikonChemVault
{
    public interface IMoleculeAPI
    {
        public Task<MoleculeBase> GetMoleculeById(Guid RegistrationId, IDictionary<string, string> headers);
        public Task<List<MoleculeBase>> GetMoleculesByIds(List<Guid> registrationIds, IDictionary<string, string> headers);
        public Task<MoleculeBase> GetMoleculeBySMILES(string smiles, IDictionary<string, string> headers);
        public Task<List<SimilarMolecule>> FindSimilar(FindSimilarMoleculesQuery query, IDictionary<string, string> headers);
        public Task<List<MoleculeBase>> FindSubstructure(FindSubstructuresQuery query, IDictionary<string, string> headers);
        public Task<List<MoleculeBase>> FindByName(string name, int limit, IDictionary<string, string> headers);
        public Task<MoleculeBase> FindByNameExact(string name, IDictionary<string, string> headers);
        public Task<List<MoleculeBase>> FindByNameWithFilters(GetByNameQuery query, IDictionary<string, string> headers);
        public Task<MoleculeBase> Register(RegisterMoleculeCommand registerMoleculeCommand, IDictionary<string, string> headers);
        public Task<MoleculeBase> Update(Guid RegistrationId, UpdateMoleculeCommand command, IDictionary<string, string> headers);
        public Task<Unit> Delete(Guid RegistrationId, IDictionary<string, string> headers);

        public Task<List<MoleculeBase>> RegisterBatch(List<RegisterMoleculeCommandWithRegId> registerMoleculeCommands, IDictionary<string, string> headers);
        public Task<Unit> BatchCreateParents(IDictionary<string, string> headers);

        public Task<List<ClusterVM>> CalculateClusters(List<ClusterDTO> Molecules, double CutOff, IDictionary<string, string> headers);
    }
}