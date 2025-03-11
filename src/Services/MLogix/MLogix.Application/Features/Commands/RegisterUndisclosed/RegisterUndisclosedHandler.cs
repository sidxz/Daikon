using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.RegisterUndisclosed
{
    public class RegisterUndisclosedHandler : IRequestHandler<RegisterUndisclosedCommand, RegisterUndisclosedDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterUndisclosedHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoleculeAPI _iMoleculeAPI;

        public RegisterUndisclosedHandler(IMapper mapper, ILogger<RegisterUndisclosedHandler> logger, IMoleculeRepository moleculeRepository,
        IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler, IHttpContextAccessor httpContextAccessor, IMoleculeAPI iMoleculeAPI)
        {
            _mapper = mapper;
            _logger = logger;
            _moleculeRepository = moleculeRepository;
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler;
            _httpContextAccessor = httpContextAccessor;
            _iMoleculeAPI = iMoleculeAPI;
        }

        public async Task<RegisterUndisclosedDTO> Handle(RegisterUndisclosedCommand request, CancellationToken cancellationToken)
        {

            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            request.SetCreateProperties(request.RequestorUserId);


            var registerUndisclosedResponseDTO = new RegisterUndisclosedDTO
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                WasAlreadyRegistered = false
            };

            // Check if name is blank throw exception
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new InvalidOperationException("Name is required");
            }

            // check if molecule is already registered in chemvault
            var moleculeInChemVault = await _iMoleculeAPI.FindByNameExact(request.Name, headers);
            
            if (moleculeInChemVault != null)
            {

                throw new InvalidOperationException("Molecule Name is already registered in chemvault id: " + moleculeInChemVault.Id);
            }

            // check if name is already registered
            var molecule = await _moleculeRepository.GetByName(request.Name);
            if (molecule != null)
            {
                _logger.LogInformation("Molecule already registered in MLogix");
                registerUndisclosedResponseDTO.Id = molecule.Id;
                registerUndisclosedResponseDTO.WasAlreadyRegistered = true;
                return registerUndisclosedResponseDTO;
            }

            // Register the molecule in the database
            try
            {
                _logger.LogInformation("Creating new undisclosed molecule in our MLogix");
                var newMoleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(request);
                newMoleculeCreatedEvent.Id = request.Id;
                newMoleculeCreatedEvent.RegistrationId = Guid.NewGuid();


                // create new molecule aggregate
                var aggregate = new MoleculeAggregate(newMoleculeCreatedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(aggregate);

                return registerUndisclosedResponseDTO;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering undisclosed molecule");
                throw;
            }


          
        }
    }

}