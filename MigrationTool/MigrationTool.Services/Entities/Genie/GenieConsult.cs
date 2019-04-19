using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieConsult
    {
        public int PatientId { get; set; }

        public DateTime? ConsultDate { get; set; }

        public string History { get; set; }

        public int DoctorId { get; set; }

        public TimeSpan ConsultTime { get; set; }

        public string Diagnosis { get; set; }

        public string Plan { get; set; }

        public int Id { get; set; }

        public string Examination { get; set; }
    }
}