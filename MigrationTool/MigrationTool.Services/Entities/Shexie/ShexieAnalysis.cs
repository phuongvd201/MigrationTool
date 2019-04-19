using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "analysis")]
    public class ShexieAnalysis
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "TabDesc")]
        public string Desc { get; set; }
    }
}