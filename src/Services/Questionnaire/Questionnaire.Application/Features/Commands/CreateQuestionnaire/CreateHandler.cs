
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Questionnaire.Application.Contracts.Persistence;
using Questionnaire.Domain.Entities;

namespace Questionnaire.Application.Features.Commands.CreateQuestionnaire
{
    public class CreateHandler : IRequestHandler<CreateCommand, Domain.Entities.Questionnaire>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreateHandler> _logger;
        private readonly IQuestionnaireRepository _questionnaireRepository;

        public CreateHandler(IMapper mapper, ILogger<CreateHandler> logger, IQuestionnaireRepository questionnaireRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _questionnaireRepository = questionnaireRepository;
        }

        public async Task<Domain.Entities.Questionnaire> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            // check if questionnaire (questionnaireName) already exists; reject if it does
            request.Name = request.Name.ToUpper();
            request.SetCreateProperties(request.RequestorUserId);
            var questionnaireExists = await _questionnaireRepository.ReadQuestionnaireByName(request.Name);

            if (questionnaireExists != null)
            {
                throw new DuplicateEntityRequestException(nameof(CreateCommand), request.Name);
            }

            // set id if not set
            if (request.Id == Guid.Empty) request.Id = Guid.NewGuid();
            request.Questions ??= [];
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
                await _questionnaireRepository.CreateQuestionnaire(questionnaire);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the questionnaire with ID {QuestionnaireId}", questionnaire.Id);
                throw new RepositoryException(nameof(CreateHandler), "Error creating questionnaire", ex);
            }

            return questionnaire;

        }
    }
}