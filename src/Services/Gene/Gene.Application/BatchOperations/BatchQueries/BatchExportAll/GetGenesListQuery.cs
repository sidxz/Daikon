
using Gene.Domain.Batch;
using MediatR;

namespace Gene.Application.BatchOperations.BatchQueries.BatchExportAll
{
    public class BatchExportAllQuery : IRequest<List<GeneExport>>
    {

    }
}