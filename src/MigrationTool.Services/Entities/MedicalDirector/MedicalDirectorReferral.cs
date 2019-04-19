using System;

namespace MigrationTool.Services.Entities.MedicalDirector
{
    internal class MedicalDirectorReferral
    {
        public int PatientId { get; set; }

        public string ReferralDoctor { get; set; }

        public DateTime? ReferralDate { get; set; }
    }
}