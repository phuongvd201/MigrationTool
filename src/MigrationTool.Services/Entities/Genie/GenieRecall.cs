using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieRecall
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Reason { get; set; }

        public int RecurrenceInterval { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? DateEntered { get; set; }

        public string DoctorFullName { get; set; }

        public string PatientPhone { get; set; }

        public string LastActionDetails { get; set; }

        public DateTime? LastActionDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? NextAppointmentDate { get; set; }
    }
}