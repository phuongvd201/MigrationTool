using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "app_type")]
    public class ShexieAppointmentType
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "desc")]
        public string Name { get; set; }

        [Column(Name = "colour")]
        public int Colour { get; set; }

        [Column(Name = "duration")]
        public int Duration { get; set; }
    }
}