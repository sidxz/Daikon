
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
    public class DiscloseMoleculeBatchHandler : IRequestHandler<DiscloseMoleculeBatchCommand, IList<MoleculeVM>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DiscloseMoleculeBatchHandler> _logger;
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IEventSourcingHandler<MoleculeAggregate> _moleculeEventSourcingHandler;
        private readonly IMoleculeAPI _moleculeAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DiscloseMoleculeBatchHandler(
            IMapper mapper,
            ILogger<DiscloseMoleculeBatchHandler> logger,
            IMoleculeRepository moleculeRepository,
            IEventSourcingHandler<MoleculeAggregate> moleculeEventSourcingHandler,
            IMoleculeAPI moleculeAPI,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeEventSourcingHandler = moleculeEventSourcingHandler ?? throw new ArgumentNullException(nameof(moleculeEventSourcingHandler));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<IList<MoleculeVM>> Handle(DiscloseMoleculeBatchCommand request, CancellationToken cancellationToken)
        {
            if (request.Molecules == null || request.Molecules.Count == 0)
                throw new ArgumentException("At least one molecule disclosure request must be provided.");

            var results = new List<MoleculeVM>();
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                          ?? new Dictionary<string, string>();

            foreach (var molecule in request.Molecules)
            {
                if (string.IsNullOrWhiteSpace(molecule.Name))
                {
                    _logger.LogWarning("Skipping molecule disclosure: Name is required.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(molecule.RequestedSMILES))
                {
                    _logger.LogWarning("Skipping molecule disclosure: SMILES is required.");
                    continue;
                }

                molecule.SetUpdateProperties(molecule.RequestorUserId);

                // Fetch the molecule from the database
                var existingMolecule = await _moleculeRepository.GetMoleculeById(molecule.Id);
                if (existingMolecule == null)
                {
                    _logger.LogWarning("Molecule not found in database: {Id}", molecule.Id);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(existingMolecule.RequestedSMILES))
                {
                    _logger.LogWarning("Molecule already disclosed: {Name}", molecule.Name);
                    continue;
                }

                // Check if SMILES exists in DaikonChemVault
                var vaultMolecule = await _moleculeAPI.GetMoleculeBySMILES(molecule.RequestedSMILES, headers);
                if (vaultMolecule != null)
                {
                    _logger.LogWarning("SMILES already disclosed: {SMILES}", molecule.RequestedSMILES);
                    continue;
                }

                // Register molecule in DaikonChemVault
                var vaultRegistrationRequest = new RegisterMoleculeCommand
                {
                    Id = existingMolecule.RegistrationId,
                    Name = existingMolecule.Name,
                    SMILES = molecule.RequestedSMILES,
                    RequestorUserId = molecule.RequestorUserId
                };

                MoleculeBase registeredMolecule;
                try
                {
                    registeredMolecule = await _moleculeAPI.Register(vaultRegistrationRequest, headers);
                    if (registeredMolecule == null)
                        throw new Exception("An error occurred while registering the molecule in MolDB");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to register molecule in DaikonChemVault");
                    continue;
                }

                // Update molecule aggregate
                var moleculeDisclosedEvent = _mapper.Map<MoleculeDisclosedEvent>(molecule);
                moleculeDisclosedEvent.Id = existingMolecule.Id;
                moleculeDisclosedEvent.RegistrationId = existingMolecule.RegistrationId;
                moleculeDisclosedEvent.RequestedSMILES = molecule.RequestedSMILES;
                moleculeDisclosedEvent.SmilesCanonical = registeredMolecule.SmilesCanonical;

                try
                {
                    var aggregate = await _moleculeEventSourcingHandler.GetByAsyncId(existingMolecule.Id);
                    aggregate.DiscloseMolecule(moleculeDisclosedEvent);
                    await _moleculeEventSourcingHandler.SaveAsync(aggregate);
                }
                catch (AggregateNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Aggregate not found for molecule: {Id}", molecule.Id);
                    continue;
                }

                // Map response
                var moleculeVm = _mapper.Map<MoleculeVM>(registeredMolecule);
                moleculeVm.Id = existingMolecule.Id;
                moleculeVm.RegistrationId = existingMolecule.RegistrationId;
                results.Add(moleculeVm);
            }

            return results;
        }
    }
}
