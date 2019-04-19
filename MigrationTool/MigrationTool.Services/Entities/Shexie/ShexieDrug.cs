using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "Drugs")]
    public class ShexieDrug
    {
        [Column(Name = "ID")]
        public int Id { get; set; }

        [Column(Name = "Drug_Name")]
        public string Name { get; set; }

        [Column(Name = "Strength")]
        public float? Strength { get; set; }

        [Column(Name = "note")]
        public string Composition { get; set; }
    }
}