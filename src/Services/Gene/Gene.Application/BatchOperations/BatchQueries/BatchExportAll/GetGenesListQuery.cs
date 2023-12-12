
using Gene.Application.BatchOperations.BatchQueries.DTOs;
using MediatR;

namespace Gene.Application.BatchOperations.BatchQueries.BatchExportAll
{
    public class BatchExportAllQuery : IRequest<List<GeneExportDto>>
    {

    }
}