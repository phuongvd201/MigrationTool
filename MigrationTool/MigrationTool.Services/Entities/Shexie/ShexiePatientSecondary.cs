using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    /// <summary>
    /// Secondary patient information mapped from database table.
    /// </summary>
    [Table(Name = "Address2")]
    public class ShexiePatientSecondary
    {
        [Column(Name = "patient")]
        public int Id { get; set; }

        [Column(Name = "Net")]
        public string Email { get; set; }

        [Column(Name = "BirthCountry")]
        public int BirthCountry { get; set; }

        [Column(Name = "health_fund")]
        public int? HealthFundId { get; set; }

        [Column(Name = "member_no")]
        public string HealthFundMemberNo { get; set; }

        [Column(Name = "Com")]
        public string BackGroundInfo { get; set; }

        public ShexieCountry Country { get; set; }
    }
}