using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieAppointment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime? StartDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public string CreatedBy { get; set; }

        public int ProviderId { get; set; }

        public int PatientId { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Reason { get; set; }

        public int Duration { get; set; }
    }
}