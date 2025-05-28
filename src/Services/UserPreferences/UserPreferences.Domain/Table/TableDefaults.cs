
using CQRS.Core.Domain;

namespace UserPreferences.Domain.Table
{
    public class TableDefaults : BaseEntity
    {
        public string TableType { get; set; }  // "hit", "screen", etc
        public List<string> Columns { get; set; }
        public int Version { get; set; }

    }

}