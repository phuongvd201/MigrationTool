using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieIncomingLetter
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public DateTime LetterDate { get; set; }

        public string Sender { get; set; }

        public string Addressee { get; set; }

        public string LetterContent { get; set; }

        public string PatientFirstName { get; set; }

        public string PatientLastName { get; set; }

        public string LetterType { get; set; }

        public string FileName { get; set; }
    }
}