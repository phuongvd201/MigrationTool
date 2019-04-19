using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieTask
    {
        public string TaskFor { get; set; }

        public string Task { get; set; }

        public DateTime? TaskDate { get; set; }

        public TimeSpan TaskTime { get; set; }

        public string Creator { get; set; }

        public string Note { get; set; }

        public int ContactId { get; set; }

        public bool Completed { get; set; }

        public DateTime? DateCompleted { get; set; }

        public int PatientId { get; set; }

        public bool Read { get; set; }

        public string Patient { get; set; }

        public DateTime? DateCreated { get; set; }

        public int Id { get; set; }

        public int Version { get; set; }

        public bool Sync { get; set; }

        public bool UrgentFg { get; set; }

        public int TSK_Id_Fk { get; set; }

        public string LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }
    }
}