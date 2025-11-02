using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.Embedded.MLogix;
using MLogix.Application.DTOs.CageFusion;

namespace MLogix.Application.Contracts.Infrastructure.CageFusion
{
    public interface INuisanceAPI
    {
        public Task<NuisanceResponseDTO> GetNuisancePredictionsAsync(NuisanceRequestDTO request);
    }
}