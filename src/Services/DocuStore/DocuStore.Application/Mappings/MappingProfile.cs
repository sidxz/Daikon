
using AutoMapper;
using Daikon.Events.DocuStore;
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
            
            CreateMap<ParsedDoc, ParsedDocDeletedEvent>().ReverseMap();

            // Queries
        }
    }
}