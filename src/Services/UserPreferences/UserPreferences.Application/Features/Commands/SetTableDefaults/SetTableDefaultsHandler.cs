
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Commands.SetTableDefaults
{
    public class SetTableDefaultsHandler : IRequestHandler<SetTableDefaultsCommand, TableDefaults>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SetTableDefaultsHandler> _logger;
        private readonly ITableDefaultsRepository _repository;

        public SetTableDefaultsHandler(IMapper mapper, ILogger<SetTableDefaultsHandler> logger, ITableDefaultsRepository repository)
        {
            _mapper = mapper;
            _logger = logger;
            _repository = repository;
        }

        public async Task<TableDefaults> Handle(SetTableDefaultsCommand request, CancellationToken cancellationToken)
        {
            
            // check in repository if the table type already exists
            var existingConfig = await _repository.GetByTableTypeAsync(request.TableType);
            if (existingConfig != null)
            {
                // if it exists, update the existing config
                request.SetUpdateProperties(request.RequestorUserId);
                request.Id = existingConfig.Id;
            }
            else
            {
                // if it doesn't exist, create a new config
                request.SetCreateProperties(request.RequestorUserId);
                request.Id = Guid.NewGuid();
            }

            var entity = _mapper.Map<TableDefaults>(request);
            await _repository.ReplaceAsync(request.TableType, entity);

            return entity;
        }
    }
}