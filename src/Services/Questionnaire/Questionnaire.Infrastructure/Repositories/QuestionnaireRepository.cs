
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Questionnaire.Application.Contracts.Persistence;

namespace Questionnaire.Infrastructure.Repositories
{
    public class QuestionnaireRepository : IQuestionnaireRepository
    {
        private readonly IMongoCollection<Domain.Entities.Questionnaire> _questionnaireCollection;
        private readonly ILogger<QuestionnaireRepository> _logger;

        public QuestionnaireRepository(IConfiguration configuration, ILogger<QuestionnaireRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("QuestionnaireMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("QuestionnaireMongoDbSettings:DatabaseName"));
            _questionnaireCollection = database.GetCollection<Domain.Entities.Questionnaire>("Questionnaires");
            _questionnaireCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Questionnaire>(Builders<Domain.Entities.Questionnaire>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = true }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateQuestionnaire(Domain.Entities.Questionnaire questionnaire)
        {
            ArgumentNullException.ThrowIfNull(questionnaire);

            try
            {
                _logger.LogInformation("CreateQuestionnaire: Creating questionnaire {QuestionnaireId}, {Questionnaire}", questionnaire.Id, questionnaire.ToJson());
                await _questionnaireCollection.InsertOneAsync(questionnaire);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the questionnaire with ID {QuestionnaireId}", questionnaire.Id);
                throw new RepositoryException(nameof(QuestionnaireRepository), "Error creating questionnaire", ex);
            }
        }

        public async Task<Domain.Entities.Questionnaire> ReadQuestionnaireById(Guid id)
        {
            return await _questionnaireCollection.Find(questionnaire => questionnaire.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Domain.Entities.Questionnaire> ReadQuestionnaireByName(string name)
        {
            return await _questionnaireCollection.Find(questionnaire => questionnaire.Name == name).FirstOrDefaultAsync();
        }

        public async Task UpdateQuestionnaire(Domain.Entities.Questionnaire questionnaire)
        {
            ArgumentNullException.ThrowIfNull(questionnaire);

            try
            {
                _logger.LogInformation("UpdateQuestionnaire: Updating questionnaire {QuestionnaireId}, {Questionnaire}", questionnaire.Id, questionnaire.ToJson());
                await _questionnaireCollection.ReplaceOneAsync(q => q.Id == questionnaire.Id, questionnaire);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the questionnaire with ID {QuestionnaireId}", questionnaire.Id);
                throw new RepositoryException(nameof(QuestionnaireRepository), "Error updating questionnaire", ex);
            }
        }
        public async Task DeleteQuestionnaire(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteQuestionnaire: Deleting questionnaire {QuestionnaireId}", id);
                await _questionnaireCollection.DeleteOneAsync(questionnaire => questionnaire.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the questionnaire with ID {QuestionnaireId}", id);
                throw new RepositoryException(nameof(QuestionnaireRepository), "Error deleting questionnaire", ex);
            }
        }

        public Task<List<Domain.Entities.Questionnaire>> GetQuestionnairesList()
        {
            try
            {
                return _questionnaireCollection.Find(questionnaire => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the questionnaire list");
                throw new RepositoryException(nameof(QuestionnaireRepository), "Error getting questionnaire list", ex);
            }
        }
    }
}