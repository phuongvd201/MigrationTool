using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedLetter
    {
        public string Id { get; set; }

        public int PatientId { get; set; }

        public string FromDoctorCode { get; set; }

        public int? PrimaryRecipient { get; set; }

        public DateTime? LetterDate { get; set; }

        public DateTime? SavedDateTime { get; set; }

        public int DocumentId { get; set; }
    }
}