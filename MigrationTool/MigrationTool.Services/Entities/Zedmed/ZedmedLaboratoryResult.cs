using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedLaboratoryResult
    {
        public string Id { get; set; }

        public int PatientId { get; set; }

        public string DoctorCode { get; set; }

        public DateTime? ReportedDate { get; set; }

        public DateTime? SavedDateTime { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? CollectedDate { get; set; }

        public string Description { get; set; }

        public string EdocId { get; set; }
    }
}