using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace UserPreferences.Application.Features.Queries.ResolveTableConfig
{
    public class ResolveTableConfigVM : DocMetadata
    {
        public string Level { get; set; }
        public string TableType { get; set; }
        public Guid TableInstanceId { get; set; }
        public Guid? UserId { get; set; }
        public List<string> Columns { get; set; }
        public int Version { get; set; }
    }
}