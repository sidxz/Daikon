using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Questionnaire.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Questionnaire, Features.Commands.CreateQuestionnaire.CreateCommand>().ReverseMap();
            //CreateMap<Domain.Entities.Questionnaire, Features.Queries.GetQuestionnaire.GetQuestionnaireByNameQuery>().ReverseMap();
            CreateMap<Domain.Entities.Questionnaire, Features.Commands.UpdateQuestionnaire.UpdateCommand>().ReverseMap();
            CreateMap<Domain.Entities.Questionnaire, Domain.Entities.Questionnaire>();
            CreateMap<Domain.Entities.Question, Domain.Entities.Question>();
            CreateMap<Domain.Entities.PossibleAnswer, Domain.Entities.PossibleAnswer>();
            

        }

    }
}