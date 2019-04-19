using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieScript
    {
        public DateTime? CreationDate { get; set; }

        public string Dose { get; set; }

        public string Medication { get; set; }

        public int ScriptNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public string Repeat { get; set; }

        public int? CreatorDoctorId { get; set; }

        public string AuthorityNumber { get; set; }

        public int PatientId { get; set; }

        public string Quantity { get; set; }

        public string DrugId { get; set; }

        public string Note { get; set; }

        public string Id { get; set; }

        public string ExternalPatientId { get; set; }
    }
}