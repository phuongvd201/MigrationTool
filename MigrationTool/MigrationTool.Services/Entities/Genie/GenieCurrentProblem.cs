using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieCurrentProblem
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Problem { get; set; }

        public string Note { get; set; }

        public DateTime? DiagnosisDate { get; set; }

        public bool Confidential { get; set; }
    }
}