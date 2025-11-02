using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Domain.Entities;

namespace MLogix.Application.Contracts.Persistence
{
    public interface IMoleculePredictionRepository
    {
        Task<MoleculePredictions> GetByMoleculeIdAsync(Guid moleculeId);
        Task<List<MoleculePredictions>> GetByMoleculeIdsAsync(List<Guid> moleculeIds);
        Task UpsertAsync(MoleculePredictions prediction);
        Task UpsertBatchAsync(List<MoleculePredictions> predictions);

    }
}