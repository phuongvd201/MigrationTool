using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieReferral
    {
        public int PatientId { get; set; }

        public int ContactId { get; set; }

        public DateTime? ReferralDate { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int Duration { get; set; }

        public int Id { get; set; }

        public string ReferredTo { get; set; }
    }
}