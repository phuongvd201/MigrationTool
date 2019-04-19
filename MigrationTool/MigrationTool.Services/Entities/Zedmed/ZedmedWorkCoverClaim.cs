using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedWorkCoverClaim
    {
        public string Id { get; set; }

        public string PatientId { get; set; }

        public string InsuranceClaimNumber { get; set; }

        public string AccountPayerId { get; set; }

        public string ContactPhone { get; set; }

        public string Name { get; set; }

        public string Employer { get; set; }

        public DateTime? EntryDate { get; set; }
    }
}