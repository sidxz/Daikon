
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeHandler(IMapper mapper, ILogger<UpdateMoleculeHandler> logger,
    IMoleculeRepository moleculeRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
    IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateMoleculeCommand, UpdateMoleculeResponseDTO>
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<UpdateMoleculeHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
        private readonly IMoleculeAPI _iMoleculeAPI = iMoleculeAPI ?? throw new ArgumentNullException(nameof(IMoleculeAPI));
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public async Task<UpdateMoleculeResponseDTO> Handle(UpdateMoleculeCommand request, CancellationToken cancellationToken)
        {

            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            request.SetUpdateProperties(request.RequestorUserId);
            // First check if the molecule exists in the database
            var existingMolecule = await _moleculeRepository.GetMoleculeById(request.Id) ?? throw new InvalidOperationException("Molecule not found");

            // check if smiles is blank throw exception
            if (string.IsNullOrEmpty(request.RequestedSMILES))
            {
                throw new InvalidOperationException("SMILES is required");
            }

            var updateMoleculeResponseDTO = new UpdateMoleculeResponseDTO();

            var moleculeUpdatedEvent = new MoleculeUpdatedEvent
            {
                Id = existingMolecule.Id,
                Name = request.Name ?? existingMolecule.Name ?? "UnNamed",
                RequestedSMILES = request.RequestedSMILES,
                RegistrationId = existingMolecule.RegistrationId,
                LastModifiedById = request.RequestorUserId
            };


            //1. Check if SMILES is different from the existing molecule, then call MolDB to update the molecule
            if (existingMolecule.RequestedSMILES != request.RequestedSMILES)
            {

                _logger.LogInformation("SMILES is different from the existing molecule, then call MolDB to update the molecule");
                // register molecule in MolDB
                MoleculeBase registrationRes;
                try
                {
                    var registrationReq = new RegisterMoleculeCommand()
                    {
                        Id = Guid.NewGuid(),
                        Name = request.Name ?? "Untitled",
                        SMILES = request.RequestedSMILES,
                        DateCreated = DateTime.UtcNow,
                        IsModified = false
                    };

                    registrationRes = await _iMoleculeAPI.Register(registrationReq, headers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while registering the molecule in MolDB");
                    throw new InvalidOperationException("An error occurred while registering the molecule in MolDB");
                }

                if (registrationRes == null)
                {
                    _logger.LogError("An error occurred while registering the molecule in MolDB");
                    throw new InvalidOperationException("An error occurred while registering the molecule in MolDB");
                }

                // now check if molecule already exists in our system
                var newRegisteredMolecule = await _moleculeRepository.GetMoleculeByRegistrationId(registrationRes.Id);


                // if molecule already exists in our system and is not the same molecule, reject the update
                // The same molecule can be returned from MolDB if the requested SMILES resolves to same canonical SMILES
                // of previous entry.
                if (newRegisteredMolecule != null)
                {
                    // check if the molecule is the same molecule
                    if (newRegisteredMolecule.Id != existingMolecule.Id)
                    {
                        _logger.LogInformation("Molecule already exists with name {name}", newRegisteredMolecule.Name);
                        throw new InvalidOperationException("Molecule already exists with name {newRegisteredMolecule.Name}");
                    }
                }

                // Delete the existing molecule in molDB
                if (newRegisteredMolecule == null || existingMolecule.RegistrationId != newRegisteredMolecule.RegistrationId)
                    try
                    {
                        await _iMoleculeAPI.Delete(existingMolecule.RegistrationId, headers);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while deleting the molecule in MolDB");
                        throw new InvalidOperationException("An error occurred while deleting the molecule in MolDB");
                    }

                // update the registrationId in the event
                moleculeUpdatedEvent.RegistrationId = registrationRes.Id;
                moleculeUpdatedEvent.SmilesCanonical = registrationRes.SmilesCanonical;

                // return response
                updateMoleculeResponseDTO = _mapper.Map<UpdateMoleculeResponseDTO>(registrationRes);
                updateMoleculeResponseDTO.WasAlreadyRegistered = false;
                // fix Ids
                updateMoleculeResponseDTO.RegistrationId = registrationRes.Id;
                updateMoleculeResponseDTO.Id = request.Id;
            }
            else
            {
                // if SMILES is the same, then get the canonical SMILES from MolDB
                // Update name and synonyms of molecule in ChemVault

                try
                {

                    var vaultMolecule = await _iMoleculeAPI.Update(existingMolecule.RegistrationId, request, headers);
                    moleculeUpdatedEvent.SmilesCanonical = vaultMolecule.SmilesCanonical;

                    // return response
                    updateMoleculeResponseDTO = _mapper.Map<UpdateMoleculeResponseDTO>(vaultMolecule);
                    updateMoleculeResponseDTO.WasAlreadyRegistered = true;
                    // fix Ids
                    updateMoleculeResponseDTO.RegistrationId = vaultMolecule.Id;
                    updateMoleculeResponseDTO.Id = request.Id;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while getting the molecule from MolDB");
                    throw new InvalidOperationException("An error occurred while getting the molecule from MolDB");
                }
            }


            try
            {
                var aggregate = await _moleculeEventSourcingHandler.GetByAsyncId(existingMolecule.Id);
                aggregate.UpdateMolecule(moleculeUpdatedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(MoleculeAggregate), request.Id);
            }

            return updateMoleculeResponseDTO;

        }
    }
}