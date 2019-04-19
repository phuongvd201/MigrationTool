using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "kin")]
    public class ShexieKin
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "cname")]
        public string FirstName { get; set; }

        [Column(Name = "sname")]
        public string LastName { get; set; }

        [Column(Name = "hphone")]
        public string HomePhone { get; set; }

        [Column(Name = "mphone")]
        public string MobilePhone { get; set; }

        [Column(Name = "add1")]
        public string Address { get; set; }

        [Column(Name = "Title")]
        public string Salutation { get; set; }
    }
}