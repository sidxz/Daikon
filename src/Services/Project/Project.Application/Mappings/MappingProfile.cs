
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Project;
using Project.Application.Features.Commands.DeleteProjectCompoundEvolution;
using Project.Application.Features.Commands.DeleteProject;
using Project.Application.Features.Commands.NewProjectCompoundEvolution;
using Project.Application.Features.Commands.NewProject;
using Project.Application.Features.Commands.UpdateProjectCompoundEvolution;
using Project.Application.Features.Commands.UpdateProject;
using Project.Application.Features.Queries.GetProject;
using Project.Application.Features.Queries.GetProjectList;
using Project.Application.DTOs.MLogixAPI;
using Project.Domain.Entities;

namespace Project.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            /* Self */
            CreateMap<Domain.Entities.Project, Domain.Entities.Project>();
            CreateMap<Domain.Entities.ProjectCompoundEvolution, Domain.Entities.ProjectCompoundEvolution>();

            /* Commands */
            CreateMap<ProjectCreatedEvent, NewProjectCommand>().ReverseMap();
            CreateMap<ProjectUpdatedEvent, UpdateProjectCommand>().ReverseMap();
            CreateMap<ProjectDeletedEvent, DeleteProjectCommand>().ReverseMap();

            CreateMap<ProjectCompoundEvolutionAddedEvent, NewProjectCompoundEvolutionCommand>().ReverseMap();
            CreateMap<ProjectCompoundEvolutionUpdatedEvent, UpdateProjectCompoundEvolutionCommand>().ReverseMap();
            CreateMap<ProjectCompoundEvolutionDeletedEvent, DeleteProjectCompoundEvolutionCommand>().ReverseMap();


            /* Events */
            CreateMap<Domain.Entities.Project, ProjectCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Project, ProjectUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Project, ProjectDeletedEvent>().ReverseMap();

            CreateMap<Domain.Entities.ProjectCompoundEvolution, ProjectCompoundEvolutionAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.ProjectCompoundEvolution, ProjectCompoundEvolutionUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.ProjectCompoundEvolution, ProjectCompoundEvolutionDeletedEvent>().ReverseMap();

            /* Queries */
            CreateMap<Domain.Entities.Project, ProjectListVM>().ReverseMap();

            CreateMap<Domain.Entities.Project, ProjectVM>()
            // Mapping DVariable<string> types
            .ForMember(dest => dest.Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Description)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Status)))
            .ForMember(dest => dest.Stage, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Stage)))

            // Mapping DVariable<Guid> types
            .ForMember(dest => dest.PrimaryOrgId, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<Guid>, Guid>(src => src.PrimaryOrgId)))

            // Mapping DVariable<DateTime> types
            .ForMember(dest => dest.H2LPredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.H2LPredictedStart)))
            .ForMember(dest => dest.H2LStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.H2LStart)))
            .ForMember(dest => dest.LOPredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.LOPredictedStart)))
            .ForMember(dest => dest.LOStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.LOStart)))
            .ForMember(dest => dest.SPPredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.SPPredictedStart)))
            .ForMember(dest => dest.SPStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.SPStart)))
            .ForMember(dest => dest.INDPredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.INDPredictedStart)))
            .ForMember(dest => dest.INDStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.INDStart)))
            .ForMember(dest => dest.P1PredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.P1PredictedStart)))
            .ForMember(dest => dest.P1Start, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.P1Start)))
            .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.CompletionDate)))
            .ForMember(dest => dest.ProjectRemovedDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.ProjectRemovedDate)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Priority)))
            .ForMember(dest => dest.Probability, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Probability)))
            .ForMember(dest => dest.PriorityNote, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.PriorityNote)))
            .ForMember(dest => dest.ProbabilityNote, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.ProbabilityNote)))
            .ForMember(dest => dest.PPLastStatusDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.PPLastStatusDate)))
            .ForMember(dest => dest.PmPriority, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.PmPriority)))
            .ForMember(dest => dest.PmProbability, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.PmProbability)))
            .ForMember(dest => dest.PmPriorityNote, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.PmPriorityNote)))
            .ForMember(dest => dest.PmProbabilityNote, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.PmProbabilityNote)))
            .ForMember(dest => dest.PmPPLastStatusDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.PmPPLastStatusDate)))
            .ReverseMap();


             CreateMap<ProjectCompoundEvolution, CompoundEvolutionVM>()
            .ForMember(dest => dest.EvolutionDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<DateTime>, DateTime>(src => src.EvolutionDate)))
            .ForMember(dest => dest.Stage, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.Stage)))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.Notes)))
            .ForMember(dest => dest.MIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.MIC)))
            .ForMember(dest => dest.MICUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.MICUnit)))
            .ForMember(dest => dest.IC50, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.IC50)))
            .ForMember(dest => dest.IC50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.IC50Unit)))
            .ForMember(dest => dest.RequestedSMILES, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<string>, string>(src => src.RequestedSMILES)))
            .ForMember(dest => dest.IsStructureDisclosed, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProjectCompoundEvolution, IValueProperty<bool>, bool>(src => src.IsStructureDisclosed)))
            .ReverseMap();

            CreateMap<GetMoleculesResultDTO, MoleculeVM>().ReverseMap();
            CreateMap<RegisterMoleculeResponseDTO, MoleculeVM>().ReverseMap();
            
        }
    }
}