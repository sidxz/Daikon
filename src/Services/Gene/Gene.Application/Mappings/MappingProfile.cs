using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Gene.Application.Features.Command.NewGene;


namespace Gene.Application.Mappings
{
    public class MappingProfile : Profile
    {
        protected MappingProfile()
        {
            CreateMap<NewGeneCommand, Domain.Entities.Gene>().ReverseMap();
        }
    }
}