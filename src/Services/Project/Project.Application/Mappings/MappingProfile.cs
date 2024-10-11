
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
using Project.Domain.Entities;
using Project.Application.Features.Commands.UpdateProjectAssociation;
using Project.Application.Features.Batch;
using Project.Application.Features.Commands.RenameProject;

namespace Project.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            /* ====== Project Core ====== */
            // -- Commands --
            CreateMap<Domain.Entities.Project, Domain.Entities.Project>();

            CreateMap<ProjectCreatedEvent, NewProjectCommand>().ReverseMap();
            CreateMap<ProjectUpdatedEvent, UpdateProjectCommand>().ReverseMap();
            CreateMap<ProjectDeletedEvent, DeleteProjectCommand>().ReverseMap();
             CreateMap<ProjectRenamedEvent, RenameProjectCommand>().ReverseMap();
            CreateMap<ProjectAssociationUpdatedEvent, UpdateProjectAssociationCommand>().ReverseMap();

            CreateMap<ProjectCreatedEvent, Domain.Entities.Project>().ReverseMap();
            CreateMap<ProjectUpdatedEvent, Domain.Entities.Project>().ReverseMap();
            CreateMap<ProjectDeletedEvent, Domain.Entities.Project>().ReverseMap();
            CreateMap<ProjectRenamedEvent, Domain.Entities.Project>().ReverseMap();
            CreateMap<ProjectAssociationUpdatedEvent, Domain.Entities.Project>().ReverseMap();

             // -- Batch --
            CreateMap<ImportOneCommand, ProjectCreatedEvent>().ReverseMap();

            // -- Queries --
            CreateMap<Domain.Entities.Project, ProjectListVM>().ReverseMap();

            CreateMap<Domain.Entities.Project, ProjectVM>()
            // Mapping DVariable<string> types
            .ForMember(dest => dest.Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Description)))
            .ForMember(dest => dest.H2LDescription, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.H2LDescription)))
            .ForMember(dest => dest.LODescription, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.LODescription)))
            .ForMember(dest => dest.SPDescription, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.SPDescription)))
            .ForMember(dest => dest.INDDescription, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.INDDescription)))
            .ForMember(dest => dest.P1Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.P1Description)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Status)))
            .ForMember(dest => dest.Stage, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.Stage)))
            .ForMember(dest => dest.PrimaryOrgId, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<Guid>, Guid>(src => src.PrimaryOrgId)))
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



            /* ====== Project Compound Evolution ====== */
            // -- Commands --

            CreateMap<ProjectCompoundEvolution, ProjectCompoundEvolution>();

            CreateMap<ProjectCompoundEvolutionAddedEvent, NewProjectCompoundEvolutionCommand>().ReverseMap();
            CreateMap<ProjectCompoundEvolutionUpdatedEvent, UpdateProjectCompoundEvolutionCommand>().ReverseMap();
            CreateMap<ProjectCompoundEvolutionDeletedEvent, DeleteProjectCompoundEvolutionCommand>().ReverseMap();

            CreateMap<ProjectCompoundEvolutionAddedEvent, ProjectCompoundEvolution>().ReverseMap();
            CreateMap<ProjectCompoundEvolutionUpdatedEvent, ProjectCompoundEvolution>().ReverseMap();
            CreateMap<ProjectCompoundEvolutionDeletedEvent, ProjectCompoundEvolution>().ReverseMap();



            // -- Queries --
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


        }
    }
}