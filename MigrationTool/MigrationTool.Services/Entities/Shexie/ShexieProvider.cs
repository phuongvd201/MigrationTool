using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "providers")]
    public class ShexieProvider
    {
        [Column(Name = "provider_no")]
        public string Provider { get; set; }

        [Column(Name = "dr_name")]
        public string ProviderName { get; set; }

        [Column(Name = "CName")]
        public string FirstName { get; set; }

        [Column(Name = "SName")]
        public string Surname { get; set; }

        [Column(Name = "Title")]
        public string Salutation { get; set; }
    }
}