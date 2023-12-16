
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


            /* Events */
            CreateMap<Domain.Entities.Screen, ScreenCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Screen, ScreenUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, HitCollectionCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitCollection, HitCollectionUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Hit, HitAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Hit, HitUpdatedEvent>().ReverseMap();


            

        }

    }
}