using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Questionnaire.Application.Contracts.Persistence;

namespace Questionnaire.Application.Features.Commands.UpdateQuestionnaire
{
    public class UpdateHandler : IRequestHandler<UpdateCommand, Domain.Entities.Questionnaire>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IQuestionnaireRepository _questionnaireRepository;

        public UpdateHandler(IMapper mapper, ILogger<UpdateHandler> logger, IQuestionnaireRepository questionnaireRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _questionnaireRepository = questionnaireRepository;
        }

        public async Task<Domain.Entities.Questionnaire> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            // fetch the existing questionnaire
            request.SetUpdateProperties(request.RequestorUserId);
            request.Name = request.Name.ToUpper();
            var existingQuestionnaire = await _questionnaireRepository.ReadQuestionnaireByName(request.Name)
                        ?? throw new AggregateNotFoundException(nameof(UpdateCommand));

            // Add ids to new questions and possible answers
            foreach (var question in request.Questions)
            {
                // if question id is not set then set it
                if (question.Id == Guid.Empty) question.Id = Guid.NewGuid();
                question.Identification = question.Identification.ToUpper();

                foreach (var possibleAns in question.PossibleAnswers)
                {
                    // if option id is not set then set it
                    if (possibleAns.Id == Guid.Empty) possibleAns.Id = Guid.NewGuid();
                    if (possibleAns.QuestionId == Guid.Empty) possibleAns.QuestionId = question.Id;
                }
            }

            var questionnaire = _mapper.Map<Domain.Entities.Questionnaire>(request);
            try
            {
                await _questionnaireRepository.UpdateQuestionnaire(questionnaire);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the questionnaire with ID {QuestionnaireId}", questionnaire.Id);
                throw new RepositoryException(nameof(UpdateHandler), "Error updating questionnaire", ex);
            }

            return questionnaire;

        }
    }
}