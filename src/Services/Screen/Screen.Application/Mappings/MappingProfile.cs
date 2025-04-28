
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Screens;
using Daikon.Shared.VM.Screen;
using Screen.Application.Features.Batch.ImportOne;
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
using Screen.Domain.Entities;

namespace Screen.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            /* ====== Screen Run ====== */
            // -- Commands --
            CreateMap<ScreenRun, ScreenRun>();

            CreateMap<ScreenRunAddedEvent, NewScreenRunCommand>().ReverseMap();
            CreateMap<ScreenRunUpdatedEvent, UpdateScreenRunCommand>().ReverseMap();
            CreateMap<ScreenRunDeletedEvent, DeleteScreenRunCommand>().ReverseMap();

            CreateMap<ScreenRun, ScreenRunAddedEvent>().ReverseMap();
            CreateMap<ScreenRun, ScreenRunUpdatedEvent>().ReverseMap();
            CreateMap<ScreenRun, ScreenRunDeletedEvent>().ReverseMap();

            // -- Queries --
            CreateMap<ScreenRun, ScreenRunVM>()
                .ForMember(dest => dest.Library, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.Library)))
                .ForMember(dest => dest.Protocol, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.Protocol)))
                .ForMember(dest => dest.LibrarySize, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.LibrarySize)))
                .ForMember(dest => dest.Scientist, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.Scientist)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<DateTime>, DateTime>(src => src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<DateTime>, DateTime>(src => src.EndDate)))
                .ForMember(dest => dest.UnverifiedHitCount, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.UnverifiedHitCount)))
                .ForMember(dest => dest.HitRate, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.HitRate)))
                .ForMember(dest => dest.PrimaryHitCount, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.PrimaryHitCount)))
                .ForMember(dest => dest.ConfirmedHitCount, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.ConfirmedHitCount)))
                .ForMember(dest => dest.NoOfCompoundsScreened, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.NoOfCompoundsScreened)))
                .ForMember(dest => dest.Concentration, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.Concentration)))
                .ForMember(dest => dest.ConcentrationUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.ConcentrationUnit)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<ScreenRun, IValueProperty<string>, string>(src => src.Notes)))
                .ReverseMap();





            /* Views */
            CreateMap<Hit, Features.Views.GetHitProperties.HitPropertiesVM>().ReverseMap();



            /* Import */
            CreateMap<NewScreenCommand, ImportOneCommand>().ReverseMap();
            CreateMap<NewScreenRunCommand, ScreenRunDTO>().ReverseMap();
            CreateMap<NewHitCollectionCommand, HitCollection>().ReverseMap();
            CreateMap<NewHitCommand, HitDTO>().ReverseMap();

            /* Commands */
            CreateMap<ScreenCreatedEvent, NewScreenCommand>().ReverseMap();
            CreateMap<ScreenUpdatedEvent, UpdateScreenCommand>().ReverseMap();
            CreateMap<ScreenDeletedEvent, DeleteScreenCommand>().ReverseMap();
            CreateMap<ScreenAssociatedTargetsUpdatedEvent, UpdateScreenAssociatedTargetsCommand>().ReverseMap();
            CreateMap<ScreenRenamedEvent, RenameScreenCommand>().ReverseMap();



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



            CreateMap<Domain.Entities.HitCollection, HitCollectionCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, HitCollectionUpdatedEvent>().ReverseMap();

            CreateMap<Domain.Entities.Hit, HitAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Hit, HitUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Hit, HitMoleculeUpdatedEvent>().ReverseMap();

            /* Queries */

            CreateMap<Domain.Entities.Screen, ScreensListVM>().ReverseMap();

            CreateMap<Domain.Entities.Screen, ScreenVM>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.Status)))
                .ForMember(dest => dest.ExpectedStartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<DateTime>, DateTime>(src => src.ExpectedStartDate)))
                .ForMember(dest => dest.ExpectedCompleteDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<DateTime>, DateTime>(src => src.ExpectedCompleteDate)))
                .ForMember(dest => dest.LatestStatusChangeDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<DateTime>, DateTime>(src => src.LatestStatusChangeDate)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.PrimaryOrgId, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<Guid>, Guid>(src => src.PrimaryOrgId)))
                .ForMember(dest => dest.PrimaryOrgName, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Screen, IValueProperty<string>, string>(src => src.PrimaryOrgName)))
                .ReverseMap();




            CreateMap<HitCollection, HitCollectionVM>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitCollection, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitCollection, IValueProperty<string>, string>(src => src.Owner)))
                .ReverseMap();

            CreateMap<Hit, HitVM>()
                /* Library Information */
                .ForMember(dest => dest.Library, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.Library)))
                .ForMember(dest => dest.LibrarySource, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.LibrarySource)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.Method)))

                /* Assay */
                .ForMember(dest => dest.AssayType, opt => opt.MapFrom(src => src.AssayType))

                /* Standard Measurements */
                .ForMember(dest => dest.IC50, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.IC50)))
                .ForMember(dest => dest.IC50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.IC50Unit)))
                .ForMember(dest => dest.EC50, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.EC50)))
                .ForMember(dest => dest.EC50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.EC50Unit)))
                .ForMember(dest => dest.Ki, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.Ki)))
                .ForMember(dest => dest.KiUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.KiUnit)))
                .ForMember(dest => dest.Kd, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.Kd)))
                .ForMember(dest => dest.KdUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.KdUnit)))
                .ForMember(dest => dest.MIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.MIC)))
                .ForMember(dest => dest.MICUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.MICUnit)))
                .ForMember(dest => dest.MICCondition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.MICCondition)))
                .ForMember(dest => dest.MIC90, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.MIC90)))
                .ForMember(dest => dest.MIC90Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.MIC90Unit)))
                .ForMember(dest => dest.MIC90Condition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.MIC90Condition)))
                .ForMember(dest => dest.GI50, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.GI50)))
                .ForMember(dest => dest.GI50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.GI50Unit)))
                .ForMember(dest => dest.TGI, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.TGI)))
                .ForMember(dest => dest.TGIUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.TGIUnit)))
                .ForMember(dest => dest.LD50, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.LD50)))
                .ForMember(dest => dest.LD50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.LD50Unit)))

                /* Dose Responses */
                .ForMember(dest => dest.DoseResponses, opt => opt.MapFrom(src => src.DoseResponses))

                /* Grouping and Notes */
                .ForMember(dest => dest.ClusterGroup, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<int>, int>(src => src.ClusterGroup)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.Notes)))

                /* Voting System */
                .ForMember(dest => dest.Positive, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<int>, int>(src => src.Positive)))
                .ForMember(dest => dest.Neutral, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<int>, int>(src => src.Neutral)))
                .ForMember(dest => dest.Negative, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<int>, int>(src => src.Negative)))
                .ForMember(dest => dest.IsVotingAllowed, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<bool>, bool>(src => src.IsVotingAllowed)))
                .ForMember(dest => dest.Voters, opt => opt.MapFrom(src => src.Voters))

                /* Molecule Info */
                .ForMember(dest => dest.RequestedSMILES, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<string>, string>(src => src.RequestedSMILES)))
                .ForMember(dest => dest.IsStructureDisclosed, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hit, IValueProperty<bool>, bool>(src => src.IsStructureDisclosed)))
                .ForMember(dest => dest.RequestedMoleculeName, opt => opt.MapFrom(src => src.RequestedMoleculeName))
                .ForMember(dest => dest.MoleculeId, opt => opt.MapFrom(src => src.MoleculeId))
                .ForMember(dest => dest.MoleculeRegistrationId, opt => opt.MapFrom(src => src.MoleculeRegistrationId))
                .ForMember(dest => dest.Molecule, opt => opt.Ignore()) // usually filled separately after Molecule fetch

                /* Reverse Mapping */
                .ReverseMap();


        }

    }
}