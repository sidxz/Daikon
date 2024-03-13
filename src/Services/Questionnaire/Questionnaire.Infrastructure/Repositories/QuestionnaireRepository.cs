// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace Questionnaire.Infrastructure.Repositories
// {
//     public class QuestionnaireRepository
//     {
//         private readonly IMongoCollection<Domain.Entities.Target> _targetCollection;
//         private readonly ILogger<TargetRepository> _logger;
//         private readonly IVersionHub<TargetRevision> _versionHub;

//         public QuestionnaireRepository(IConfiguration configuration, ILogger<TargetRepository> logger, IVersionHub<TargetRevision> versionMaintainer)
//         {
//             var client = new MongoClient(configuration.GetValue<string>("TargetMongoDbSettings:ConnectionString"));
//             var database = client.GetDatabase(configuration.GetValue<string>("TargetMongoDbSettings:DatabaseName"));
//             _targetCollection = database.GetCollection<Domain.Entities.Target>(configuration.GetValue<string>("TargetMongoDbSettings:TargetCollectionName"));
//             _targetCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Target>(Builders<Domain.Entities.Target>.IndexKeys.Ascending(t => t.Name), new CreateIndexOptions { Unique = false }));
//             _targetCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Target>(Builders<Domain.Entities.Target>.IndexKeys.Ascending(t => t.StrainId), new CreateIndexOptions { Unique = false }));

//             _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

//             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//         }
//     }
// }