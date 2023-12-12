


using Gene.Application.BatchOperations.BatchQueries.DTOs;

namespace Gene.Application.Contracts.Infrastructure
{
    public interface IBatchRepositoryOperations
    {
        Task<List<GeneExportDto>> GetAll();
    }
}