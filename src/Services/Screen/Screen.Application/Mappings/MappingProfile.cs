
using AutoMapper;
using Daikon.Events.Screens;

namespace Screen.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            /* Commands */
            CreateMap<Domain.Entities.Screen, Features.Commands.NewScreen.NewScreenCommand>().ReverseMap();
            CreateMap<Domain.Entities.Screen, Features.Commands.UpdateScreen.UpdateScreenCommand>().ReverseMap();
            CreateMap<Domain.Entities.Screen, Features.Commands.DeleteScreen.DeleteScreenCommand>().ReverseMap();

            CreateMap<Domain.Entities.ScreenRun, Features.Commands.NewScreenRun.NewScreenRunCommand>().ReverseMap();
            CreateMap<Domain.Entities.ScreenRun, Features.Commands.UpdateScreenRun.UpdateScreenRunCommand>().ReverseMap();
            CreateMap<Domain.Entities.ScreenRun, Features.Commands.DeleteScreenRun.DeleteScreenRunCommand>().ReverseMap();

            CreateMap<Domain.Entities.HitCollection, Features.Commands.NewHitCollection.NewHitCollectionCommand>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, Features.Commands.UpdateHitCollection.UpdateHitCollectionCommand>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, Features.Commands.DeleteHitCollection.DeleteHitCollectionCommand>().ReverseMap();

            CreateMap<Domain.Entities.Hit, Features.Commands.NewHit.NewHitCommand>().ReverseMap();
            CreateMap<Domain.Entities.Hit, Features.Commands.UpdateHit.UpdateHitCommand>().ReverseMap();
            CreateMap<Domain.Entities.Hit, Features.Commands.DeleteHit.DeleteHitCommand>().ReverseMap();


            /* Events */
            CreateMap<Domain.Entities.Screen, ScreenCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Screen, ScreenUpdatedEvent>().ReverseMap();

            CreateMap<Domain.Entities.ScreenRun, ScreenRunAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.ScreenRun, ScreenRunUpdatedEvent>().ReverseMap();

            CreateMap<Domain.Entities.HitCollection, HitCollectionCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, HitCollectionUpdatedEvent>().ReverseMap();

            CreateMap<Domain.Entities.Hit, HitAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Hit, HitUpdatedEvent>().ReverseMap();

        }

    }
}