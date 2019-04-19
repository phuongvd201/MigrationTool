using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "country")]
    public class ShexieCountry
    {
        [Column(Name = "code")]
        public int Id { get; set; }

        [Column(Name = "country")]
        public string CountryName { get; set; }
    }
}