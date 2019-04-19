using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "Alarms")]
    public class ShexieAlarm
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "adate")]
        public DateTime? DueDate { get; set; }

        [Column(Name = "atime")]
        public DateTime? DueTime { get; set; }

        [Column(Name = "Completed")]
        public byte IsFinished { get; set; }

        [Column(Name = "patient")]
        public int? PatientId { get; set; }

        [Column(Name = "Oper")]
        public string ToUserId { get; set; }

        [Column(Name = "Sender")]
        public string FromUserId { get; set; }

        [Column(Name = "message")]
        public string Description { get; set; }
    }
}