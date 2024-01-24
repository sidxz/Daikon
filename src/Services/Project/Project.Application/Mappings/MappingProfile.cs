
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

namespace Project.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
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
            .ForMember(dest => dest.ProjectStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.ProjectStart)))
            .ForMember(dest => dest.ProjectPredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<DateTime>, DateTime>(src => src.ProjectPredictedStart)))
            .ForMember(dest => dest.ProjectDescription, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.ProjectDescription)))
            .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.ProjectStatus)))
            .ForMember(dest => dest.PrimaryOrg, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Project, IValueProperty<string>, string>(src => src.PrimaryOrg)))

            .ReverseMap();
        }
    }
}