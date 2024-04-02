using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Questionnaire.Application.Contracts.Persistence;

namespace Questionnaire.Application.Features.Queries.ListQuestionnaires
{
    public class ListQuestionnairesHandler : IRequestHandler<ListQuestionnaireQuery, List<Domain.Entities.Questionnaire>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ListQuestionnairesHandler> _logger;
        private readonly IQuestionnaireRepository _questionnaireRepository;

        public ListQuestionnairesHandler(IMapper mapper, ILogger<ListQuestionnairesHandler> logger, IQuestionnaireRepository questionnaireRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _questionnaireRepository = questionnaireRepository;
        }

        public async Task<List<Domain.Entities.Questionnaire>> Handle(ListQuestionnaireQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ListQuestionnairesHandler.Handle - Retrieving all questionnaires");
            try
            {
                var questionnaires = await _questionnaireRepository.GetQuestionnairesList();
                return questionnaires;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the questionnaires");
                throw new RepositoryException(nameof(ListQuestionnairesHandler), "Error retrieving questionnaires", ex);
            }
        }
    }
}