using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "accounts")]
    public class ShexieCompany : ISuburbInfo
    {
        [Column(Name = "account")]
        public int Id { get; set; }

        [Column(Name = "add1")]
        public string AddressLine1 { get; set; }

        [Column(Name = "add2")]
        public string AddressLine2 { get; set; }

        [Column(Name = "name")]
        public string Name { get; set; }

        [Column(Name = "phone")]
        public string Phone { get; set; }

        [Column(Name = "fax")]
        public string Fax { get; set; }

        [Column(Name = "suburbid")]
        public int? SuburbId { get; set; }

        [Column(Name = "tag")]
        public string HealthFundName { get; set; }

        [Column(Name = "FundBrand")]
        public string HealthFundCode { get; set; }

        public string Suburb { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }
    }
}