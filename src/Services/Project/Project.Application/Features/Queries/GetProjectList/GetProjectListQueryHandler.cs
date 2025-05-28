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

        public GetProjectListQueryHandler(
            IProjectRepository projectRepository,
            IProjectCompoundEvolutionRepository projectCompoundEvolutionRepository,
            IMLogixAPI mLogixAPIService,
            IMapper mapper,
            ILogger<GetProjectListQueryHandler> logger)
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
                // Get the list of Projects
                var projects = await _projectRepository.GetProjectList();

                // Map the Projects to ViewModels
                var projectsVM = _mapper.Map<List<ProjectListVM>>(projects);

                // Get all ProjectIds from the list
                var projectIds = projectsVM.Select(project => project.Id).ToList();

                if (projectIds == null || projectIds.Count == 0)
                {
                    return projectsVM ?? [];
                }

                // Fetch all compound evolutions for these ProjectIds in a single call
                var projectCompoundEvolutionsMap = await _projectCompoundEvolutionRepository.GetProjectCompoundEvolutionsOfProjects(projectIds);

                // Update each ProjectListVM with the latest compound evolution
                foreach (var project in projectsVM)
                {
                    if (projectCompoundEvolutionsMap.TryGetValue(project.Id, out var projectCompoundEvolutions))
                    {
                        // Get the latest compound evolution for this ProjectId
                        var latest = projectCompoundEvolutions.LastOrDefault();
                        if (latest != null)
                        {
                            project.CompoundEvoLatestSMILES = latest.RequestedSMILES;
                        }
                    }
                }

                return projectsVM;
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Error in Project Repository", ex);
            }
        }
    }
}
