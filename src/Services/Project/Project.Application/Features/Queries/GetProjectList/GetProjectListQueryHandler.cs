
using AutoMapper;
using Project.Application.Contracts.Persistence;
using MediatR;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Daikon.Shared.APIClients.MLogix;


namespace Project.Application.Features.Queries.GetProjectList
{
    public class GetProjectListQueryHandler : IRequestHandler<GetProjectListQuery, List<ProjectListVM>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvolutionRepository;
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProjectListQueryHandler> _logger;

        public GetProjectListQueryHandler(IProjectRepository projectRepository, 
        IProjectCompoundEvolutionRepository projectCompoundEvolutionRepository, 
        IMLogixAPI mLogixAPIService, IMapper mapper, ILogger<GetProjectListQueryHandler> logger)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectCompoundEvolutionRepository = projectCompoundEvolutionRepository ?? throw new ArgumentNullException(nameof(projectCompoundEvolutionRepository));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectListVM>> Handle(GetProjectListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projects = await _projectRepository.GetProjectList();
                var projectsVM = _mapper.Map<List<ProjectListVM>>(projects);
                foreach (var project in projectsVM)
                {
                    // fetch compound evolution of HA. This returns a list of compound evolutions
                    var haCompoundEvo = await _projectCompoundEvolutionRepository.GetProjectCompoundEvolutionOfProject(project.Id);
                    var latest = haCompoundEvo.LastOrDefault();
                    if (latest != null)
                    {
                        project.CompoundEvoLatestSMILES = latest.RequestedSMILES;
                    }
                }
                return projectsVM;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in HA Repository", ex);
            }


        }
    }
}