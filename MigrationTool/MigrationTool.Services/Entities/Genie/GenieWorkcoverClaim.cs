using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieWorkCoverClaim
    {
        public string Id { get; set; }

        public string PatientId { get; set; }

        public string ClaimNum { get; set; }

        public DateTime? InjuryDate { get; set; }

        public TimeSpan? InjuryTime { get; set; }

        public string Injury { get; set; }

        public string EmployerId { get; set; }

        public string InsurerId { get; set; }

        public string InjuryMechanism { get; set; }

        public string Location { get; set; }

        public string CaseManagerName { get; set; }

        public string CaseManagerWorkPhone { get; set; }

        public string CaseManagerMobilePhone { get; set; }
    }
}