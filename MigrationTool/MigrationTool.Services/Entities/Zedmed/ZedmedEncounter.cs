using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedEncounter
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string DoctorCode { get; set; }

        public DateTime? StartDateTime { get; set; }

        public string ConvertedData { get; set; }
    }
}