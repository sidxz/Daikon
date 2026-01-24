using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Features.Queries.GetUndisclosedMolecules.ByName
{
    public class GetUndisclosedMoleculesByNameHandler : IRequestHandler<GetUndisclosedMoleculesByNameQuery, List<MoleculeVM>>
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUndisclosedMoleculesByNameHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUndisclosedMoleculesByNameHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<GetUndisclosedMoleculesByNameHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _moleculeRepository = moleculeRepository;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<MoleculeVM>> Handle(GetUndisclosedMoleculesByNameQuery request, CancellationToken cancellationToken)
        {
            // check if the query list is empty
            if (request.Names == null || !request.Names.Any())
            {
                _logger.LogWarning("GetUndisclosedMoleculesByNameQuery received with empty Names list.");
                return [];
            }

            // Fetch molecules by names
            var molecules = await _moleculeRepository.GetByNames(request.Names);
            // undisclosed molecules are those with IsStructureDisclosed == false or RequestedSMILES field null/empty/whitespace
            var undisclosedMolecules = molecules.Where(m => !m.IsStructureDisclosed || string.IsNullOrWhiteSpace(m.RequestedSMILES)).ToList();

            return _mapper.Map<List<MoleculeVM>>(undisclosedMolecules);
        }
    }
}