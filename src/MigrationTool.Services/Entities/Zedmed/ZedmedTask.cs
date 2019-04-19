using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedTask
    {
        public int? Id { get; set; }

        public string UserName { get; set; }

        public string EnteredBy { get; set; }

        public string PatientId { get; set; }

        public DateTime? EnteredDateTime { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? LastPerformedDateTime { get; set; }

        public string Comments { get; set; }
    }
}