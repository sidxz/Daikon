
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;

namespace UserStore.Application.Features.Commands.Roles.DeleteRole
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteRoleHandler> _logger;
        private readonly IAppRoleRepository _appRoleRepository;

        public DeleteRoleHandler(IMapper mapper, ILogger<DeleteRoleHandler> logger, IAppRoleRepository appRoleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appRoleRepository = appRoleRepository;
        }
        public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            // check if role exists
            var existingRole = await _appRoleRepository.GetRoleById(request.Id);
            if (existingRole == null)
            {
                throw new ResourceNotFoundException(nameof(DeleteRoleHandler), $"Role with ID {request.Id} not found.");
            }

            try
            {
                await _appRoleRepository.DeleteRole(existingRole.Id);
                _logger.LogInformation("DeleteRole: Role {RoleId} deleted", existingRole.Id);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the role with ID {RoleId}", existingRole.Id);
                throw;
            }
        }
    }
}