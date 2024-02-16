
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Users.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteUserHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;

        public DeleteUserHandler(IMapper mapper, ILogger<DeleteUserHandler> logger, IAppUserRepository appUserRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
        }



        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Fetch user
            var user = await _appUserRepository.GetUserById(request.Id);

            if (user == null)
            {
                throw new ResourceNotFoundException(nameof(AppUser), request.Id);
            }

            // Delete user
            try
            {
                await _appUserRepository.DeleteUser(request.Id);
                return Unit.Value;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to delete user {user.Id}: {e.Message}");
            }

        }
    }
}