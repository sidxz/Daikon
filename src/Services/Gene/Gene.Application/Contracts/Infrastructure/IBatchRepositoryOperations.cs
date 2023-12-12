

using Gene.Domain.Batch;

namespace Gene.Application.Contracts.Infrastructure
{
    public interface IBatchRepositoryOperations
    {
        Task<List<GeneExport>> GetAll();
    }
}