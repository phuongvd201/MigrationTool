using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GeniePastHistory
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string History { get; set; }

        public string Note { get; set; }

        public DateTime? CreationDate { get; set; }

        public bool Confidential { get; set; }
    }
}