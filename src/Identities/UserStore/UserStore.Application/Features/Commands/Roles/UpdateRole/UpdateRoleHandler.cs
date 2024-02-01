
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Roles.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, AppRole>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleHandler> _logger;
        private readonly IAppRoleRepository _appRoleRepository;

        public UpdateRoleHandler(IMapper mapper, ILogger<UpdateRoleHandler> logger, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appRoleRepository = appRoleRepository;
        }
        public async Task<AppRole> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            // find role by id
            var existingRole = await _appRoleRepository.GetRoleById(request.Id);
            if (existingRole == null)
            {
                throw new ResourceNotFoundException(nameof(UpdateRoleHandler), $"Role with ID {request.Id} not found.");
            }

            // if role name is changed, check if new name is unique
            if (existingRole.Name != request.Name)
            {
                var roleWithSameName = await _appRoleRepository.GetRoleByName(request.Name);
                if (roleWithSameName != null)
                {
                    throw new DuplicateEntityRequestException(nameof(UpdateRoleHandler), $"Role with name {request.Name} already exists.");
                }
            }

            // update role
            var updatedRole = _mapper.Map(request, existingRole);

            try
            {
                await _appRoleRepository.UpdateRole(updatedRole);
                _logger.LogInformation($"Role {updatedRole.Name} was updated.");
                return updatedRole;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error updating role {updatedRole.Name}.");
                throw;
            }
        }
    }
}