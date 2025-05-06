
using CQRS.Core.Domain;

namespace UserPreferences.Domain.Table
{
    public class TableGlobalConfig : BaseEntity
    {
        public string TableType { get; set; }
        public Guid TableInstanceId { get; set; }
        public List<string> Columns { get; set; }
        public int Version { get; set; } 

    }

}