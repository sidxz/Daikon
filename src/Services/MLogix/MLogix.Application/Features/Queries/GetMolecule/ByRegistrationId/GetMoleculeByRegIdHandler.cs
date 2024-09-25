
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId
{
    public class GetMoleculeByRegIdHandler(IMoleculeRepository moleculeRepository, IMapper mapper,
    ILogger<GetMoleculeByRegIdHandler> logger, IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetMoleculeByRegIdQuery, MoleculeVM>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetMoleculeByRegIdHandler> _logger = logger;

        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<MoleculeVM> Handle(GetMoleculeByRegIdQuery request, CancellationToken cancellationToken)
        {

            var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(request.RegistrationId)
                                ?? throw new ResourceNotFoundException(nameof(GetMoleculeByRegIdHandler), request.RegistrationId);

            var moleculeVm = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);

            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

                var vaultMolecule = await _iMoleculeAPI.GetMoleculeById(molecule.RegistrationId, headers);

                _mapper.Map(vaultMolecule, moleculeVm);
                // Fix Ids
                moleculeVm.RegistrationId = vaultMolecule.Id;
                moleculeVm.Id = molecule.Id;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMoleculeByRegId");
                return moleculeVm;
            }
            return moleculeVm;
        }
    }
}