
using AutoMapper;
using CQRS.Core.Exceptions;
using Project.Application.Contracts.Persistence;
using MediatR;

namespace Project.Application.Features.Queries.GetProject.ById
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectVM>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;


        public GetProjectByIdQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }
        public async Task<ProjectVM> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            
            var project = await _projectRepository.ReadProjectById(request.Id);

            if (project == null)
            {
                throw new ResourceNotFoundException(nameof(Project), request.Id);
            }

            var projectVm = _mapper.Map<ProjectVM>(project, opts => opts.Items["WithMeta"] = request.WithMeta);

            return projectVm;

        }
    }
}