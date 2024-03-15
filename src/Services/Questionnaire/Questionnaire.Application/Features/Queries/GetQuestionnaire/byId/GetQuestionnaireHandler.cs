using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Questionnaire.Application.Contracts.Persistence;

namespace Questionnaire.Application.Features.Queries.GetQuestionnaire.ById
{
    public class GetQuestionnaireHandler : IRequestHandler<GetQuestionnaireQuery, Domain.Entities.Questionnaire>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetQuestionnaireHandler> _logger;
        private readonly IQuestionnaireRepository _questionnaireRepository;

        public GetQuestionnaireHandler(IMapper mapper, ILogger<GetQuestionnaireHandler> logger, IQuestionnaireRepository questionnaireRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _questionnaireRepository = questionnaireRepository;
        }

        public Task<Domain.Entities.Questionnaire> Handle(GetQuestionnaireQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetQuestionnaireHandler.Handle - Retrieving questionnaire by Id: {QuestionnaireId}", request.Id);
            try
            {

                var questionnaire = _questionnaireRepository.ReadQuestionnaireById(request.Id) 
                    ?? throw new AggregateNotFoundException(nameof(GetQuestionnaireQuery));
                return questionnaire;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the questionnaire with name {QuestionnaireId}", request.Id);
                throw new RepositoryException(nameof(GetQuestionnaireHandler), "Error retrieving questionnaire", ex);
            }
        }
    }
}