using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedImageDocument
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string FileExtension { get; set; }

        public DateTime? SavedDateTime { get; set; }

        public string ImageDesc { get; set; }
    }
}