
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Infrastructure;
using Gene.Domain.Batch;
using MediatR;

namespace Gene.Application.Features.Queries.BatchExportAll
{
    public class BatchExportAllQueryHandler : IRequestHandler<BatchExportAllQuery, List<GeneExport>>
    {
        private readonly IBatchRepositoryOperations _batchRepositoryOperations;
        

        public BatchExportAllQueryHandler(IBatchRepositoryOperations batchRepositoryOperations)
        {
            _batchRepositoryOperations = batchRepositoryOperations;
        }
        public async Task<List<GeneExport>> Handle(BatchExportAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var geneExportList = await _batchRepositoryOperations.GetAll();
                
                return geneExportList;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in BatchRepositoryOperations", ex);
            }
        }
    }
}