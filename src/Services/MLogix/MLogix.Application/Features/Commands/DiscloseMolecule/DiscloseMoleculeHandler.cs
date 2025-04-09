
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using Daikon.Shared.VM.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.DiscloseMolecule
{
    public class DiscloseMoleculeHandler(IMapper mapper, ILogger<DiscloseMoleculeHandler> logger,
    IMoleculeRepository moleculeRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
    IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DiscloseMoleculeCommand, MoleculeVM>
    {

        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<DiscloseMoleculeHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMoleculeRepository _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
        private readonly IMoleculeAPI _MoleculeAPI = iMoleculeAPI ?? throw new ArgumentNullException(nameof(IMoleculeAPI));
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        private const string ErrorMoleculeNotFound = "Molecule not found";
        private const string ErrorAlreadyDisclosed = "The molecule has already been disclosed";
        private const string ErrorSMILESRequired = "SMILES is required";
        private const string ErrorNameRequired = "Name is required";
        private const string ErrorSMILESAlreadyDisclosed = "The SMILES has already been disclosed with the name: {0}";
        private const string ErrorMoleculeRegistration = "An error occurred while registering the molecule in MolDB";


        public async Task<MoleculeVM> Handle(DiscloseMoleculeCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException(ErrorNameRequired);

            if (string.IsNullOrWhiteSpace(request.RequestedSMILES))
                throw new ArgumentException(ErrorSMILESRequired);


            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            request.SetUpdateProperties(request.RequestorUserId);



            // Fetch the molecule from the database
            var existingMolecule = await _moleculeRepository.GetMoleculeById(request.Id) ?? throw new ResourceNotFoundException(nameof(MoleculeAggregate), request.Id);
            if (!string.IsNullOrWhiteSpace(existingMolecule.RequestedSMILES))
                throw new InvalidOperationException(ErrorAlreadyDisclosed);


            // Now search in the DaikonChemVault
            var vaultMolecule = await _MoleculeAPI.GetMoleculeBySMILES(request.RequestedSMILES, headers);
            if (vaultMolecule != null)
                throw new InvalidOperationException(string.Format(ErrorSMILESAlreadyDisclosed, vaultMolecule.Name));


            // Register molecule in vault
            var vaultRegistrationRequest = new RegisterMoleculeCommand
            {
                Id = existingMolecule.RegistrationId,
                Name = existingMolecule.Name,
                SMILES = request.RequestedSMILES,
                RequestorUserId = request.RequestorUserId
            };
            MoleculeBase registeredMolecule;
            try
            {
                registeredMolecule = await _MoleculeAPI.Register(vaultRegistrationRequest, headers);
                if (registeredMolecule == null)
                    throw new Exception(ErrorMoleculeRegistration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMoleculeRegistration);
                throw new InvalidOperationException(ErrorMoleculeRegistration);
            }


            // Now update the aggregate
            var moleculeDisclosedEvent = _mapper.Map<MoleculeDisclosedEvent>(request);
            moleculeDisclosedEvent.Id = existingMolecule.Id;
            moleculeDisclosedEvent.RegistrationId = existingMolecule.RegistrationId;
            moleculeDisclosedEvent.RequestedSMILES = request.RequestedSMILES;
            moleculeDisclosedEvent.SmilesCanonical = registeredMolecule.SmilesCanonical;


            try
            {
                var aggregate = await _moleculeEventSourcingHandler.GetByAsyncId(existingMolecule.Id);
                aggregate.DiscloseMolecule(moleculeDisclosedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(MoleculeAggregate), request.Id);
            }

            // Map the response
            var moleculeVm = _mapper.Map<MoleculeVM>(registeredMolecule);
            moleculeVm.Id = existingMolecule.Id;
            moleculeVm.RegistrationId = existingMolecule.RegistrationId;

            return moleculeVm;
        }
    }
}