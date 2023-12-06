
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrainsList
{
    public class GetStrainsListQueryHandler : IRequestHandler<GetStrainsListQuery, List<StrainsListVM>>
    {
        private readonly IStrainRepository _strainRepository;

        public GetStrainsListQueryHandler(IStrainRepository strainRepository)
        {
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
        }
        public async Task<List<StrainsListVM>> Handle(GetStrainsListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var strainsList = await _strainRepository.GetStrainsList();
                return strainsList.Select(strain => new StrainsListVM
                {
                    Id = strain.Id,
                    Name = strain.Name,
                    Organism = strain.Organism,
                    
                }).ToList();
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Strain Repository", ex);
            }


        }
    }
}