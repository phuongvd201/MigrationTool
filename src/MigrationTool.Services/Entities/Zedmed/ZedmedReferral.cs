using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedReferral
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int? ReferralDoctortId { get; set; }

        public DateTime? ReferralDate { get; set; }

        public DateTime? LetterDate { get; set; }

        public int ReferralPeriod { get; set; }
    }
}