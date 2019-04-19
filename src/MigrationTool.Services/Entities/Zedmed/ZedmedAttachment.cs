using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedAttachment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string FileName { get; set; }

        public DateTime? SavedDateTime { get; set; }

        public string Description { get; set; }
    }
}