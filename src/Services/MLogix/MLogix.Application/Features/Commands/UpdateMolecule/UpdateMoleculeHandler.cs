using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.MolDbAPI;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeHandler : IRequestHandler<UpdateMoleculeCommand, UpdateMoleculeResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMoleculeHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMolDbAPIService _molDbAPIService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateMoleculeHandler(IMapper mapper, ILogger<UpdateMoleculeHandler> logger, IMoleculeRepository moleculeRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler, IMolDbAPIService molDbAPIService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _molDbAPIService = molDbAPIService ?? throw new ArgumentNullException(nameof(molDbAPIService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<UpdateMoleculeResponseDTO> Handle(UpdateMoleculeCommand request, CancellationToken cancellationToken)
        {

            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            request.DateModified = DateTime.UtcNow;
            request.IsModified = true;
            // First check if the molecule exists in the database
            var existingMolecule = await _moleculeRepository.GetMoleculeById(request.Id) ?? throw new InvalidOperationException("Molecule not found");

            // check if smiles is blank throw exception
            if (string.IsNullOrEmpty(request.RequestedSMILES))
            {
                throw new InvalidOperationException("SMILES cannot be blank");
            }

            var registerMoleculeResponseDTO = new UpdateMoleculeResponseDTO();

            var moleculeUpdatedEvent = new MoleculeUpdatedEvent
            {
                Id = existingMolecule.Id,
                Name = request.Name ?? existingMolecule.Name ?? "UnNamed",
                RequestedSMILES = request.RequestedSMILES,
                Synonyms = request.Synonyms ?? [],
                Ids = request.Ids ?? [],
                RegistrationId = existingMolecule.RegistrationId,
                LastModifiedById = request.RequestorUserId
            };


            //1. Check if SMILES is different from the existing molecule, then call MolDB to update the molecule
            if (existingMolecule.RequestedSMILES != request.RequestedSMILES)
            {

                // register molecule in MolDB
                MoleculeDTO registrationReq;
                try
                {
                    registrationReq = await _molDbAPIService.RegisterCompound(request.Name ?? "UnNamed", request.RequestedSMILES, headers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while registering the molecule in MolDB");
                    throw new InvalidOperationException("An error occurred while registering the molecule in MolDB");
                }

                if (registrationReq == null)
                {
                    _logger.LogError("An error occurred while registering the molecule in MolDB");
                    throw new InvalidOperationException("An error occurred while registering the molecule in MolDB");
                }

                // now check if molecule already exists in our system
                var newRegisteredMolecule = await _moleculeRepository.GetMoleculeByRegistrationId(registrationReq.Id);
                

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
                    await _molDbAPIService.DeleteMoleculeById(existingMolecule.RegistrationId, headers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the molecule in MolDB");
                    throw new InvalidOperationException("An error occurred while deleting the molecule in MolDB");
                }

                // update the registrationId in the event
                moleculeUpdatedEvent.RegistrationId = registrationReq.Id;
                moleculeUpdatedEvent.SmilesCanonical = registrationReq.SmilesCanonical;
            }
            else
            {
                // if SMILES is the same, then get the canonical SMILES from MolDB

                try
                {
                    var molDbMolecule = await _molDbAPIService.GetMoleculeById(existingMolecule.RegistrationId, headers);
                    moleculeUpdatedEvent.SmilesCanonical = molDbMolecule.SmilesCanonical;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while getting the molecule from MolDB");
                    throw new InvalidOperationException("An error occurred while getting the molecule from MolDB");
                }
            }


            try
            {
                var aggregate = await _moleculeEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateMolecule(moleculeUpdatedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(MoleculeAggregate), request.Id);
            }

            return _mapper.Map<UpdateMoleculeResponseDTO>(moleculeUpdatedEvent);

        }
    }
}