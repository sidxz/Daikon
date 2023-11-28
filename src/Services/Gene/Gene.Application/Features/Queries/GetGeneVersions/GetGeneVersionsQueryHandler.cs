using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using Gene.Application.Features.Queries.GetGene;
using MediatR;

namespace Gene.Application.Features.Queries.GetGeneVersions.ById
{
    public class GetGeneVersionsQueryHandler : IRequestHandler<GetGeneVersionsQuery, GeneVersionsVM>
    {
        private readonly IGeneRepository _geneRepository;

        public GetGeneVersionsQueryHandler(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
        }
        public async Task<GeneVersionsVM> Handle(GetGeneVersionsQuery request, CancellationToken cancellationToken)
        {
            var geneRevision = await _geneRepository.GetGeneRevisions(request.Id);

            if (geneRevision == null)
            {
                throw new ResourceNotFoundException(nameof(Gene), request.Id);
            }

            return new GeneVersionsVM
            {
                Id = geneRevision.Id,
                Function = geneRevision.Function,
                Product = geneRevision.Product,
                FunctionalCategory = geneRevision.FunctionalCategory
            };



            
        }
    }
}