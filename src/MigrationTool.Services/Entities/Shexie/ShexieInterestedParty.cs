using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "interested_doctors")]
    public class ShexieInterestedParty
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "service_id")]
        public int ServiceId { get; set; }

        [Column(Name = "other_doctor")]
        public int ContactId { get; set; }

        public int? PatientId { get; set; }
    }
}