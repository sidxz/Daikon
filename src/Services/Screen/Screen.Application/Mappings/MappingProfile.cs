
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Screens;
using Screen.Application.Features.Commands.DeleteHit;
using Screen.Application.Features.Commands.DeleteHitCollection;
using Screen.Application.Features.Commands.DeleteScreen;
using Screen.Application.Features.Commands.DeleteScreenRun;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Commands.NewHitCollection;
using Screen.Application.Features.Commands.NewScreen;
using Screen.Application.Features.Commands.NewScreenRun;
using Screen.Application.Features.Commands.RenameHitCollection;
using Screen.Application.Features.Commands.RenameScreen;
using Screen.Application.Features.Commands.UpdateHit;
using Screen.Application.Features.Commands.UpdateHitCollection;
using Screen.Application.Features.Commands.UpdateHitCollectionAssociatedScreen;
using Screen.Application.Features.Commands.UpdateScreen;
using Screen.Application.Features.Commands.UpdateScreenAssociatedTargets;
using Screen.Application.Features.Commands.UpdateScreenRun;

namespace Screen.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            /* Commands */
            CreateMap<ScreenCreatedEvent, NewScreenCommand>().ReverseMap();
            CreateMap<ScreenUpdatedEvent, UpdateScreenCommand>().ReverseMap();
            CreateMap<ScreenDeletedEvent, DeleteScreenCommand>().ReverseMap();
            CreateMap<ScreenAssociatedTargetsUpdatedEvent, UpdateScreenAssociatedTargetsCommand>().ReverseMap();
            CreateMap<ScreenRenamedEvent, RenameScreenCommand>().ReverseMap();

            CreateMap<ScreenRunAddedEvent, NewScreenRunCommand>().ReverseMap();
            CreateMap<ScreenRunUpdatedEvent, UpdateScreenRunCommand>().ReverseMap();
            CreateMap<ScreenRunDeletedEvent, DeleteScreenRunCommand>().ReverseMap();

            CreateMap<HitCollectionCreatedEvent, NewHitCollectionCommand>().ReverseMap();
            CreateMap<HitCollectionUpdatedEvent, UpdateHitCollectionCommand>().ReverseMap();
            CreateMap<HitCollectionDeletedEvent, DeleteHitCollectionCommand>().ReverseMap();
            CreateMap<HitCollectionAssociatedScreenUpdatedEvent, UpdateHitCollectionAssociatedScreenCommand>().ReverseMap();
            CreateMap<HitCollectionRenamedEvent, RenameHitCollectionCommand>().ReverseMap();

            CreateMap<HitAddedEvent, NewHitCommand>().ReverseMap();
            CreateMap<HitUpdatedEvent, UpdateHitCommand>().ReverseMap();
            CreateMap<HitDeletedEvent, DeleteHitCommand>().ReverseMap();


            /* Events */
            CreateMap<Domain.Entities.Screen, ScreenCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Screen, ScreenUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Screen, ScreenDeletedEvent>().ReverseMap();

            CreateMap<Domain.Entities.ScreenRun, ScreenRunAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.ScreenRun, ScreenRunUpdatedEvent>().ReverseMap();

            CreateMap<Domain.Entities.HitCollection, HitCollectionCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, HitCollectionUpdatedEvent>().ReverseMap();

            CreateMap<Domain.Entities.Hit, HitAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Hit, HitUpdatedEvent>().ReverseMap();

            /* Queries */

            CreateMap<Domain.Entities.Screen, Features.Queries.ViewModels.ScreensListVM>().ReverseMap();

            CreateMap<Domain.Entities.Screen, Features.Queries.ViewModels.ScreenVM>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.Status)))
                .ForMember(dest => dest.ExpectedCompleteDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<DateTime>, DateTime>(src => src.ExpectedCompleteDate)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.PrimaryOrgId, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<Guid>, Guid>(src => src.PrimaryOrgId)))
                .ForMember(dest => dest.PrimaryOrgName, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.PrimaryOrgName)))
                .ReverseMap();

            CreateMap<Domain.Entities.ScreenRun, Features.Queries.ViewModels.ScreenRunVM>()
                .ForMember(dest => dest.Library, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<string>, string>(src => src.Library)))
                .ForMember(dest => dest.Protocol, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<string>, string>(src => src.Protocol)))
                .ForMember(dest => dest.LibrarySize, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<int>, int>(src => src.LibrarySize)))
                .ForMember(dest => dest.Scientist, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<string>, string>(src => src.Scientist)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<DateTime>, DateTime>(src => src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<DateTime>, DateTime>(src => src.EndDate)))
                .ForMember(dest => dest.UnverifiedHitCount, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<int>, int>(src => src.UnverifiedHitCount)))
                .ForMember(dest => dest.HitRate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<double>, double>(src => src.HitRate)))
                .ForMember(dest => dest.PrimaryHitCount, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<int>, int>(src => src.PrimaryHitCount)))
                .ForMember(dest => dest.ConfirmedHitCount, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<int>, int>(src => src.ConfirmedHitCount)))
                .ForMember(dest => dest.NoOfCompoundsScreened, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<int>, int>(src => src.NoOfCompoundsScreened)))
                .ForMember(dest => dest.Concentration, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<string>, string>(src => src.Concentration)))
                .ForMember(dest => dest.ConcentrationUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<string>, string>(src => src.ConcentrationUnit)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ScreenRun, IValueProperty<string>, string>(src => src.Notes)))
                .ReverseMap();


            CreateMap<Domain.Entities.HitCollection, Features.Queries.ViewModels.HitCollectionVM>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitCollection, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitCollection, IValueProperty<string>, string>(src => src.Owner)))
                .ReverseMap();

            CreateMap<Domain.Entities.Hit, Features.Queries.ViewModels.HitVM>()
                .ForMember(dest => dest.Library, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.Library)))
                .ForMember(dest => dest.LibrarySource, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.LibrarySource)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.MIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.MIC)))
                .ForMember(dest => dest.MICUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.MICUnit)))
                .ForMember(dest => dest.MICCondition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.MICCondition)))
                .ForMember(dest => dest.IC50, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.IC50)))
                .ForMember(dest => dest.IC50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.IC50Unit)))
                .ForMember(dest => dest.ClusterGroup, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<int>, int>(src => src.ClusterGroup)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.Positive, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<int>, int>(src => src.Positive)))
                .ForMember(dest => dest.Neutral, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<int>, int>(src => src.Neutral)))
                .ForMember(dest => dest.Negative, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<int>, int>(src => src.Negative)))
                .ForMember(dest => dest.IsVotingAllowed, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<bool>, bool>(src => src.IsVotingAllowed)))
                .ForMember(dest => dest.RequestedSMILES, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<string>, string>(src => src.RequestedSMILES)))
                .ForMember(dest => dest.IsStructureDisclosed, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hit, IValueProperty<bool>, bool>(src => src.IsStructureDisclosed)))
                .ReverseMap();

        }

    }
}