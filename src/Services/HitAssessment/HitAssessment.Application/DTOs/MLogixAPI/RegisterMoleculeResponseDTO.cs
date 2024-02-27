using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HitAssessment.Application.DTOs.MolDbAPI;

namespace HitAssessment.Application.DTOs.MLogixAPI
{
    public class RegisterMoleculeResponseDTO
    {
        public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }
        public string? Name { get; set; }
        public CompoundDTO? Calculated { get; set; }
    }
}