
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrain.ByName
{
    public class GetStrainByNameQueryHandler : IRequestHandler<GetStrainByNameQuery, StrainVM>
    {
        private readonly IStrainRepository _strainRepository;

        public GetStrainByNameQueryHandler(IStrainRepository strainRepository)
        {
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
        }
        public async Task<StrainVM> Handle(GetStrainByNameQuery request, CancellationToken cancellationToken)
        {
            var strain = await _strainRepository.ReadStrainByName(request.Name);

            if (strain == null)
            {
                throw new ResourceNotFoundException(nameof(GetStrainByNameQuery), request.Name);
            }

            return new StrainVM
            {
                Id = strain.Id,
                Name = strain.Name,
                Organism = strain.Organism,
            };
        }
    }
}