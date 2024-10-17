
using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.MLogix;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Domain.Aggregates;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeHandler : IRequestHandler<RegisterMoleculeCommand, RegisterMoleculeResponseDTO>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<RegisterMoleculeHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMoleculeAPI _iMoleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RegisterMoleculeHandler(IMapper mapper, ILogger<RegisterMoleculeHandler> logger,
        IMoleculeRepository moleculeRepository, IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
        IMoleculeAPI iMoleculeAPI, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _iMoleculeAPI = iMoleculeAPI ?? throw new ArgumentNullException(nameof(_iMoleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<RegisterMoleculeResponseDTO> Handle(RegisterMoleculeCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            request.SetCreateProperties(request.RequestorUserId);

            var registerMoleculeResponseDTO = new RegisterMoleculeResponseDTO();

            // check if smiles is blank throw exception
            if (string.IsNullOrEmpty(request.SMILES))
            {
                throw new InvalidOperationException("SMILES is required");
            }

            // register molecule in MolDB
            MoleculeBase registrationReq;
            try
            {
                registrationReq = await _iMoleculeAPI.Register(request, headers);
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
            var molecule = await _moleculeRepository.GetMoleculeByRegistrationId(registrationReq.Id);
            if (molecule != null)
            {
                _logger.LogInformation("Molecule already exists in our system, returning existing molecule");
                registerMoleculeResponseDTO = _mapper.Map<RegisterMoleculeResponseDTO>(registrationReq);
                registerMoleculeResponseDTO.WasAlreadyRegistered = true;
                // fix Ids
                registerMoleculeResponseDTO.RegistrationId = registrationReq.Id;
                registerMoleculeResponseDTO.Id = molecule.Id;

                return registerMoleculeResponseDTO;
            }

            // create new molecule

            try
            {
                _logger.LogInformation("Creating new molecule in our system");
                var newMoleculeCreatedEvent = _mapper.Map<MoleculeCreatedEvent>(request);
                newMoleculeCreatedEvent.Id = request.Id;
                newMoleculeCreatedEvent.RegistrationId = registrationReq.Id;
                newMoleculeCreatedEvent.RequestedSMILES = request.SMILES;
                newMoleculeCreatedEvent.SmilesCanonical = registrationReq.SmilesCanonical;

                // create new molecule aggregate
                var aggregate = new MoleculeAggregate(newMoleculeCreatedEvent);
                await _moleculeEventSourcingHandler.SaveAsync(aggregate);

                // return response
                registerMoleculeResponseDTO = _mapper.Map<RegisterMoleculeResponseDTO>(registrationReq);
                registerMoleculeResponseDTO.WasAlreadyRegistered = false;
                // fix Ids
                registerMoleculeResponseDTO.RegistrationId = registrationReq.Id;
                registerMoleculeResponseDTO.Id = request.Id;


                return registerMoleculeResponseDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling the molecule in our system");
                throw;
            }

        }
    }
}