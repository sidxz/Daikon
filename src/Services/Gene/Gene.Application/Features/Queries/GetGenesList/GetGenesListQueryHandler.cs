using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetGenesList
{
    public class GetGenesListQueryHandler : IRequestHandler<GetGenesListQuery, List<GenesListVM>>
    {
        private readonly IGeneRepository _geneRepository;

        public GetGenesListQueryHandler(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
        }
        public async Task<List<GenesListVM>> Handle(GetGenesListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var geneList = await _geneRepository.GetGenesList();
                return geneList.Select(gene => new GenesListVM
                {
                    Id = gene.Id,
                    AccessionNumber = gene.AccessionNumber,
                    Name = gene.Name,
                    Function = gene.Function.Value,
                    Product = gene.Product.Value,
                    FunctionalCategory = gene.FunctionalCategory.Value
                }).ToList();
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Gene Repository", ex);
            }


        }
    }
}