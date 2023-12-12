
using Gene.Domain.Batch;
using MediatR;

namespace Gene.Application.Features.Queries.BatchExportAll
{
    public class BatchExportAllQuery : IRequest<List<GeneExport>>
    {
        
    }
}