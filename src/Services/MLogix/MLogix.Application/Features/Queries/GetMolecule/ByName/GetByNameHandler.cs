
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetMolecule.ByName
{
    public class GetByNameHandler(IMoleculeRepository moleculeRepository,
        IMapper mapper, ILogger<GetByNameHandler> logger,
        IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetByNameQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetByNameHandler> _logger = logger;
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<List<MoleculeVM>> Handle(GetByNameQuery request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());
            try
            {
                _logger.LogInformation("Find Molecules matching name: {0} and limit: {2}", request.Name, request.Limit);

                var res = new List<MoleculeVM>();
                var vaultMolecules = await _iMoleculeAPI.FindByName(request.Name, request.Limit, headers);

                _logger.LogInformation("Found {0} molecules with name {1}", vaultMolecules.Count, request.Name);

                foreach (var vaultMolecule in vaultMolecules)
                {
                    try
                    {
                        var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(vaultMolecule.Id);
                        var MoleculeVM = _mapper.Map<MoleculeVM>(molecule, opts => opts.Items["WithMeta"] = request.WithMeta);
                        _mapper.Map(vaultMolecule, MoleculeVM);
                        // Fix Ids
                        MoleculeVM.RegistrationId = vaultMolecule.Id;
                        MoleculeVM.Id = molecule.Id;

                        res.Add(MoleculeVM);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in processing molecule with ID: {0}", vaultMolecule.Id);
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindSubstructures");
            }

            throw new ResourceNotFoundException(nameof(GetByNameHandler), request.Name);

        }
    }
}