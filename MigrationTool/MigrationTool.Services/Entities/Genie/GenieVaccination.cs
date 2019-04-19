using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieVaccination
    {
        public int Id { get; set; }

        public string Vaccine { get; set; }

        public int Dose { get; set; }

        public int PatientId { get; set; }

        public DateTime? GivenDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string TermCode { get; set; }

        public string ICPCCode { get; set; }

        public string ICD10Code { get; set; }

        public string ACIRCode { get; set; }
    }
}