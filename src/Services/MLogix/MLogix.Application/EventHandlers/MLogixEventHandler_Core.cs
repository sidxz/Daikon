using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.MLogix;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Entities;

namespace MLogix.Application.EventHandlers
{
    public class MLogixEventHandler : IMLogixEventHandler
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MLogixEventHandler> _logger;

        public MLogixEventHandler(IMoleculeRepository moleculeRepository, IMapper mapper, ILogger<MLogixEventHandler> logger)
        {
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnEvent(MoleculeCreatedEvent @event)
        {
            _logger.LogInformation("MoleculeCreatedEvent received for molecule {moleculeId}", @event.Id);
            var molecule = _mapper.Map<Molecule>(@event);
            molecule.Id = @event.Id;
            molecule.DateCreated = @event.DateCreated;
            molecule.IsModified = false;

            try {
                await _moleculeRepository.NewMolecule(molecule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the molecule with ID {moleculeId}", @event.Id);
                throw new EventHandlerException(nameof(MLogixEventHandler), "Error creating molecule", ex);
            }

        }

        public Task OnEvent(MoleculeUpdatedEvent @event)
        {
            throw new NotImplementedException();
        }

        public Task OnEvent(MoleculeDeletedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}