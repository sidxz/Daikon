using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Questionnaire.Application.Contracts.Persistence
{
    public interface IQuestionnaireRepository
    {
        Task<List<Domain.Entities.Questionnaire>> GetQuestionnairesList();
        Task<Domain.Entities.Questionnaire> ReadQuestionnaireById(Guid id);
        Task<Domain.Entities.Questionnaire> ReadQuestionnaireByName(string name);
        Task CreateQuestionnaire(Domain.Entities.Questionnaire questionnaire);
        Task UpdateQuestionnaire(Domain.Entities.Questionnaire questionnaire);
        Task DeleteQuestionnaire(Guid id);
        
    }
}