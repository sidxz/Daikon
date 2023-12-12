
using CQRS.Core.Exceptions;
using Gene.Application.BatchOperations.BatchQueries.DTOs;
using Gene.Application.Contracts.Infrastructure;

using MediatR;

namespace  Gene.Application.BatchOperations.BatchQueries.BatchExportAll
{
    public class BatchExportAllQueryHandler : IRequestHandler<BatchExportAllQuery, List<GeneExportDto>>
    {
        private readonly IBatchRepositoryOperations _batchRepositoryOperations;
        

        public BatchExportAllQueryHandler(IBatchRepositoryOperations batchRepositoryOperations)
        {
            _batchRepositoryOperations = batchRepositoryOperations;
        }
        public async Task<List<GeneExportDto>> Handle(BatchExportAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GeneExportDtoList = await _batchRepositoryOperations.GetAll();
                
                return GeneExportDtoList;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in BatchRepositoryOperations", ex);
            }
        }
    }
}