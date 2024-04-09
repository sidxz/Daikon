using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Application.DTOs.MLogixAPI;

namespace Project.Application.Contracts.Infrastructure
{
    public interface IMLogixAPIService
    {
        public Task<RegisterMoleculeResponseDTO> RegisterCompound(RegisterMoleculeRequest registerMoleculeRequest);
        public Task<GetMoleculesResultDTO> GetMoleculeById(Guid id);
    }
}