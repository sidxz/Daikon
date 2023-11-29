using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using Gene.Application.Features.Queries.GetGene;
using MediatR;

namespace Gene.Application.Features.Queries.GetGene.ByAccession
{
    public class GetGeneByAccessionQueryHandler : IRequestHandler<GetGeneByAccessionQuery, GeneVM>
    {
        private readonly IGeneRepository _geneRepository;

        public GetGeneByAccessionQueryHandler(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
        }
        public async Task<GeneVM> Handle(GetGeneByAccessionQuery request, CancellationToken cancellationToken)
        {
            var gene = await _geneRepository.ReadGeneByAccession(request.AccessionNumber);

            if (gene == null)
            {
                throw new ResourceNotFoundException(nameof(Gene), request.AccessionNumber);
            }

            if (request.WithMeta)
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