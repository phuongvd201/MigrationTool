using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "postcode")]
    public class ShexiePostcode
    {
        [Column(Name = "suburbid")]
        public int Id { get; set; }

        [Column(Name = "postcode")]
        public string Postcode { get; set; }

        [Column(Name = "suburb")]
        public string Suburb { get; set; }

        [Column(Name = "country")]
        public int CountryId { get; set; }

        [Column(Name = "state")]
        public string State { get; set; }

        public string Country { get; set; }
    }
}