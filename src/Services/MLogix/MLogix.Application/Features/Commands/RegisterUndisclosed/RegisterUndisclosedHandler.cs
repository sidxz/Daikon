using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Daikon.EventStore.Handlers;
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

            request.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

            var registerUndisclosedResponseDTO = new RegisterUndisclosedDTO
            {
                Id = request.Id,
                Name = request.Name,
                WasAlreadyRegistered = false
            };

            // Check if name is blank throw exception
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new InvalidOperationException("Name is required");
            }


            if (request.ChemVaultCheck)
            {
                // check if molecule is already registered in ChemVault
                var moleculeInChemVault = await _iMoleculeAPI.FindByNamesOrSynonymsExact([request.Name], headers);

                if (moleculeInChemVault.Count > 0)
                {

                    throw new InvalidOperationException("ERROR: The Molecule is already disclosed. Molecule was registered in ChemVault with id: " + moleculeInChemVault[0].Id);
                }
            }
            else
            {
                _logger.LogInformation("ChemVault uniqueness check skipped. Command.ChemVaultCheck is set to false for molecule name: {Name}", request.Name);
            }
            


            // check if name is already registered
            var molecule = await _moleculeRepository.GetByName(request.Name);
            if (molecule != null)
            {
                _logger.LogInformation("Molecule already registered in MLogix");
                registerUndisclosedResponseDTO.Id = molecule.Id;
                registerUndisclosedResponseDTO.RegistrationId = molecule.RegistrationId;
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

                registerUndisclosedResponseDTO.RegistrationId = newMoleculeCreatedEvent.RegistrationId;

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