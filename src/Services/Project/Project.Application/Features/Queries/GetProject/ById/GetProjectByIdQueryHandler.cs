
using AutoMapper;
using CQRS.Core.Exceptions;
using Project.Application.Contracts.Persistence;
using MediatR;
using Project.Application.Contracts.Infrastructure;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Project.Domain.Entities;

namespace Project.Application.Features.Queries.GetProject.ById
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectVM>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvolutionRepository;
        private readonly IMLogixAPIService _mLogixAPIService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProjectByIdQueryHandler> _logger;

        public GetProjectByIdQueryHandler(IProjectRepository projectRepository, IProjectCompoundEvolutionRepository projectCompoundEvolutionRepository, IMLogixAPIService mLogixAPIService, IMapper mapper, ILogger<GetProjectByIdQueryHandler> logger)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectCompoundEvolutionRepository = projectCompoundEvolutionRepository ?? throw new ArgumentNullException(nameof(projectCompoundEvolutionRepository));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProjectVM> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            
           // Fetch Project from db
            var project = await _projectRepository.ReadProjectById(request.Id);

            if (project == null)
            {
                throw new ResourceNotFoundException(nameof(Project), request.Id);
            }


            // map to project VM
            var projectVm = _mapper.Map<ProjectVM>(project, opts => opts.Items["WithMeta"] = request.WithMeta);


            // fetch compound evolution of Project. This returns a list of compound evolutions
            var compoundEvo = await _projectCompoundEvolutionRepository.GetProjectCompoundEvolutionOfProject(request.Id);


            // map each compound evolution to compound evolution VM
            projectVm.CompoundEvolution = _mapper.Map<List<CompoundEvolutionVM>>(compoundEvo, opts => opts.Items["WithMeta"] = request.WithMeta);



             projectVm.CompoundEvoLatestSMILES = (string)projectVm.CompoundEvolution.FirstOrDefault()?.RequestedSMILES ?? string.Empty;
            // _logger.LogInformation("Calculated CompoundEvoLatestSMILES {CompoundEvoLatestSMILES}", projectVm.CompoundEvoLatestSMILES);

            projectVm.CompoundEvoLatestMoleculeId = (Guid?)projectVm.CompoundEvolution.FirstOrDefault()?.MoleculeId ?? default(Guid);



            // fetch molecule for each compound evolution
            foreach (var evolution in projectVm.CompoundEvolution)
            {
                _logger.LogInformation("Fetching molecule for compound evolution with Molecule Id", evolution.MoleculeId);
                try
                {
                    var molecule = await _mLogixAPIService.GetMoleculeById(evolution.MoleculeId);

                    evolution.Molecule = _mapper.Map<MoleculeVM>(molecule);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching molecule for compound evolution with Molecule Id", evolution.MoleculeId);

                }
            }

            // Finally return the complete HA VM
            return projectVm;
        }
    }
}