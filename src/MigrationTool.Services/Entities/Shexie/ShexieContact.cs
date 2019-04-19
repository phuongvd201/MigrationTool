using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "referee")]
    public class ShexieContact : ISuburbInfo
    {
        [Column(Name = "ID")]
        public int Id { get; set; }

        [Column(Name = "add1")]
        public string AddressLine1 { get; set; }

        [Column(Name = "add2")]
        public string AddressLine2 { get; set; }

        [Column(Name = "sname")]
        public string LastName { get; set; }

        [Column(Name = "phone")]
        public string HomePhone { get; set; }

        [Column(Name = "mobile")]
        public string MobilePhone { get; set; }

        [Column(Name = "fax")]
        public string Fax { get; set; }

        [Column(Name = "salutation")]
        public string Salutation { get; set; }

        [Column(Name = "cname")]
        public string FirstName { get; set; }

        [Column(Name = "prov")]
        public string ProviderNo { get; set; }

        [Column(Name = "suburbid")]
        public int? SuburbId { get; set; }

        [Column(Name = "net")]
        public string Email { get; set; }

        public string Suburb { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }
    }
}