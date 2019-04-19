using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedChecklist
    {
        public string Id { get; set; }

        public int PatientId { get; set; }

        public string DoctorId { get; set; }

        public DateTime? DateCreated { get; set; }

        public string Name { get; set; }

        public string SectionNotes { get; set; }

        public int TemplateId { get; set; }
    }
}