using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieOutgoingLetter
    {
        public DateTime LetterDate { get; set; }

        public int PatientId { get; set; }

        public string Creator { get; set; }

        public string From { get; set; }

        public int ContactId { get; set; }

        public int Id { get; set; }

        public string ReferralContent { get; set; }

        public string Cda { get; set; }

        public bool Reviewed { get; set; }

        public bool PrimarySent { get; set; }

        public bool ReadyToSend { get; set; }

        public bool ReplayReceived { get; set; }
    }
}