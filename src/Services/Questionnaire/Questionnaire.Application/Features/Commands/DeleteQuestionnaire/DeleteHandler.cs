
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Questionnaire.Application.Contracts.Persistence;

namespace Questionnaire.Application.Features.Commands.DeleteQuestionnaire
{
    public class DeleteHandler : IRequestHandler<DeleteCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteHandler> _logger;
        private readonly IQuestionnaireRepository _questionnaireRepository;

        public DeleteHandler(IMapper mapper, ILogger<DeleteHandler> logger, IQuestionnaireRepository questionnaireRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _questionnaireRepository = questionnaireRepository;
        }

        public async Task<Unit> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            // fetch the existing questionnaire
            request.Name = request.Name.ToUpper();
            var existingQuestionnaire = await _questionnaireRepository.ReadQuestionnaireByName(request.Name)
                        ?? throw new AggregateNotFoundException(nameof(DeleteCommand));

            try
            {
                await _questionnaireRepository.DeleteQuestionnaire(existingQuestionnaire.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the questionnaire with ID {QuestionnaireId}", existingQuestionnaire.Id);
                throw new RepositoryException(nameof(DeleteHandler), "Error deleting questionnaire", ex);
            }

            return Unit.Value;

        }
    }
}