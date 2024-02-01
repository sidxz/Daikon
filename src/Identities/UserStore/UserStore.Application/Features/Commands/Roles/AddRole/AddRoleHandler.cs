
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Roles.AddRole
{
    public class AddRoleHandler : IRequestHandler<AddRoleCommand, AppRole>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddRoleHandler> _logger;
        private readonly IAppRoleRepository _appRoleRepository;

        public AddRoleHandler(IMapper mapper, ILogger<AddRoleHandler> logger, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appRoleRepository = appRoleRepository;
        }
        
        public async Task<AppRole> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            // check if role already exists
            var existingRole = await _appRoleRepository.GetRoleByName(request.Name);
            if (existingRole != null)
            {
                throw new DuplicateEntityRequestException(nameof(AddRoleHandler), $"Role with name {request.Name} already exists.");
            }

            var role = _mapper.Map<AppRole>(request);

            try
            {
                await _appRoleRepository.AddRole(role);
                _logger.LogInformation("AddRole: Role {RoleId} created", role.Id);
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the role with ID {RoleId}", role.Id);
                throw;
            }
        }
    }

}