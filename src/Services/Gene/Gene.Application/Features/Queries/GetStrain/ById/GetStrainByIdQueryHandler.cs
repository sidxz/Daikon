
using CQRS.Core.Exceptions;
using Gene.Application.Contracts.Persistence;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrain.ById
{
    public class GetStrainByIdQueryHandler : IRequestHandler<GetStrainByIdQuery, StrainVM>
    {
        private readonly IStrainRepository _strainRepository;

        public GetStrainByIdQueryHandler(IStrainRepository strainRepository)
        {
            _strainRepository = strainRepository ?? throw new ArgumentNullException(nameof(strainRepository));
        }
        public async Task<StrainVM> Handle(GetStrainByIdQuery request, CancellationToken cancellationToken)
        {
            var strain = await _strainRepository.ReadStrainById(request.Id);

            if (strain == null)
            {
                throw new ResourceNotFoundException(nameof(GetStrainByIdQuery), request.Id);
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