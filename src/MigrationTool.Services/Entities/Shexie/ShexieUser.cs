using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "security")]
    internal class ShexieUser
    {
        [Column(Name = "user_id")]
        public string Id { get; set; }

        [Column(Name = "user_cname")]
        public string FirstName { get; set; }

        [Column(Name = "user_sname")]
        public string Surname { get; set; }
    }
}