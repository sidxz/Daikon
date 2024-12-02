
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.DocuStore;
using Daikon.Shared.VM.DocuStore;
using DocuStore.Application.Features.Commands.AddParsedDoc;
using DocuStore.Application.Features.Commands.UpdateParsedDoc;
using DocuStore.Domain.Entities;

namespace DocuStore.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /* ====== ParseDoc Core ====== */

            // Commands
            CreateMap<ParsedDoc, ParsedDoc>();

            CreateMap<ParsedDoc, ParsedDocAddedEvent>().ReverseMap();
            CreateMap<AddParsedDocCommand, ParsedDocAddedEvent>().ReverseMap();

            CreateMap<ParsedDoc, ParsedDocUpdatedEvent>().ReverseMap();
            CreateMap<UpdateParsedDocCommand, ParsedDocUpdatedEvent>().ReverseMap();
            CreateMap<UpdateParsedDocCommand, AddParsedDocCommand>().ReverseMap();


            CreateMap<ParsedDoc, ParsedDocDeletedEvent>().ReverseMap();

            // Queries

            CreateMap<ParsedDoc, ParsedDocVM>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(new MapperDVariableMetaResolver<ParsedDoc, IValueProperty<string>, string>(src => src.Title)))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(new MapperDVariableMetaResolver<ParsedDoc, IValueProperty<string>, string>(src => src.Authors)))
            .ForMember(dest => dest.ShortSummary, opt => opt.MapFrom(new MapperDVariableMetaResolver<ParsedDoc, IValueProperty<string>, string>(src => src.ShortSummary)))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<ParsedDoc, IValueProperty<string>, string>(src => src.Notes)))
            .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<ParsedDoc, IValueProperty<DateTime>, DateTime>(src => src.PublicationDate)))
            .ReverseMap()
            ;
        }
    }
}