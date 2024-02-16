
using AutoMapper;
using Project.Application.Contracts.Persistence;
using MediatR;
using CQRS.Core.Exceptions;


namespace Project.Application.Features.Queries.GetProjectList
{
    public class GetProjectListQueryHandler : IRequestHandler<GetProjectListQuery, List<ProjectListVM>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;


        public GetProjectListQueryHandler(IProjectRepository targetRepository, IMapper mapper)
        {
            _projectRepository = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public async Task<List<ProjectListVM>> Handle(GetProjectListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var haS = await _projectRepository.GetProjectList();

                return _mapper.Map<List<ProjectListVM>>(haS);
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Project Repository", ex);
            }


        }
    }
}