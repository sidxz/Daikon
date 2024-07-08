
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Roles.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, UpdateRoleDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleHandler> _logger;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppUserRepository _appUserRepository;

        public UpdateRoleHandler(IMapper mapper, ILogger<UpdateRoleHandler> logger, IAppRoleRepository appRoleRepository, IAppUserRepository appUserRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appRoleRepository = appRoleRepository;
            _appUserRepository = appUserRepository;
        }
        public async Task<UpdateRoleDTO> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
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

            // get existing attached users to role
            var existingUsers = await _appUserRepository.GetUsersByRole(existingRole.Id);
            // check if any users have been added or removed
            var usersToAdd = request.Users.Except(existingUsers.Select(u => u.Id)).ToList();
            var usersToRemove = existingUsers.Select(u => u.Id).Except(request.Users).ToList();

            // add users to role
            foreach (var userId in usersToAdd)
            {
                var user = await _appUserRepository.GetUserById(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    continue;
                }

                user.AppRoleIds.Add(existingRole.Id);
                await _appUserRepository.UpdateUser(user);
                _logger.LogInformation($"User {user.Email} added to role {existingRole.Name}.");
            }

            // remove users from role
            foreach (var userId in usersToRemove)
            {
                var user = await _appUserRepository.GetUserById(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    continue;
                }

                user.AppRoleIds.Remove(existingRole.Id);
                await _appUserRepository.UpdateUser(user);
                _logger.LogInformation($"User {user.Email} removed from role {existingRole.Name}.");
            }

            // update role
            var updatedRole = _mapper.Map(request, existingRole);

            try
            {
                await _appRoleRepository.UpdateRole(updatedRole);
                _logger.LogInformation($"Role {updatedRole.Name} was updated.");

                var updatedRoleDTO = _mapper.Map<UpdateRoleDTO>(updatedRole);
                updatedRoleDTO.Users = request.Users;

                return updatedRoleDTO;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error updating role {updatedRole.Name}.");
                throw;
            }
        }
    }
}