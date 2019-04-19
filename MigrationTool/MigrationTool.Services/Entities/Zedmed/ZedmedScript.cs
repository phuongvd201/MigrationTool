using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedScript
    {
        public DateTime? DateFirstPrescribed { get; set; }

        public string DosageFullText { get; set; }

        public string ScriptDesc { get; set; }

        public string Repeats { get; set; }

        public string DoctorCode { get; set; }

        public string AuthorityNumber { get; set; }

        public int? PatientId { get; set; }

        public string DrugId { get; set; }

        public string Notes { get; set; }

        public string Id { get; set; }
    }
}