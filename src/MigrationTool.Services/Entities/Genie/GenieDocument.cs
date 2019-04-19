using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieDocument
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime? DocumentDate { get; set; }

        public DateTime? DateModified { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }
    }
}