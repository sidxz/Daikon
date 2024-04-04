using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistance;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Features.Calculation
{
    public class FindRoot
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<FindRoot> _logger;

        public FindRoot(IGraphQueryRepository graphQueryRepository, ILogger<FindRoot> logger)
        {
            _graphQueryRepository = graphQueryRepository;
            _logger = logger;
        }

        public static string ByUniId(string uniId)
        {

            // Case when root is gene

            var runQuery = @"MATCH (x {uniId : $uniId}) <-[rel*]-(x)
                                    WHERE x:Gene OR x:Target OR x:Screen OR x:HitCollection
                                    RETURN i, rel, x";
            var parameters = new Dictionary<string, object> { { "uniId", "6c013bbb-eec9-4197-80d1-4c23bc04ff46" } };



            return "root";
        }
    }
}