using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "PathoTests")]
    public class ShexieLaboratoryResultType
    {
        [Column(Name = "Test_Id")]
        public int Id { get; set; }

        [Column(Name = "Test_Desc")]
        public string Description { get; set; }
    }
}