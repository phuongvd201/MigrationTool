using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedPatientProblem
    {
        public string Id { get; set; }

        public int PatientId { get; set; }

        public string ProblemText { get; set; }

        public DateTime? OnsetDate { get; set; }
    }
}