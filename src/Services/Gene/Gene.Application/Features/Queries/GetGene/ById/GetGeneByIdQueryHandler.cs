using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Gene.Application.Contracts.Persistence;
using Gene.Application.Features.Queries.GetGene;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ById
{
    public class GetGeneByIdQueryHandler : IRequestHandler<GetGeneByIdQuery, GeneVM>
    {
        private readonly IGeneRepository _geneRepository;

        public GetGeneByIdQueryHandler(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
        }
        public async Task<GeneVM> Handle(GetGeneByIdQuery request, CancellationToken cancellationToken)
        {
            var gene = await _geneRepository.ReadGeneById(request.Id);

            if (request.IncludeMetadata)
            {
                return new GeneVM
                {
                    Id = gene.Id,
                    Name = gene.Name,
                    AccessionNumber = gene.AccessionNumber,
                    Function = gene.Function,
                    Product = gene.Product,
                    FunctionalCategory = gene.FunctionalCategory
                };
            }

            return new GeneVM
            {
                Id = gene.Id,
                Name = gene.Name,
                AccessionNumber = gene.AccessionNumber,
                Function = gene.Function.Value,
                Product = gene.Product.Value,
                FunctionalCategory = gene.FunctionalCategory.Value
            };
        }
    }
}