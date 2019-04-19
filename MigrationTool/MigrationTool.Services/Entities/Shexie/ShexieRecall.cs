using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "diary")]
    public class ShexieRecall
    {
        [Column(Name = "cnt")]
        public int Id { get; set; }

        [Column(Name = "patient")]
        public int PatientId { get; set; }

        [Column(Name = "Reason")]
        public string Reason { get; set; }

        [Column(Name = "repeats")]
        public int RecurrenceInterval { get; set; }

        [Column(Name = "date")]
        public DateTime Date { get; set; }

        [Column(Name = "days")]
        public byte Days { get; set; }

        [Column(Name = "weeks")]
        public byte Weeks { get; set; }

        [Column(Name = "months")]
        public byte Months { get; set; }

        [Column(Name = "years")]
        public byte Years { get; set; }

        [Column(Name = "Due")]
        public DateTime? DueDate { get; set; }

        [Column(Name = "provider")]
        public string Provider { get; set; }

        [Column(Name = "cancel")]
        public bool IsCancel { get; set; }

        [Column(Name = "NoFU")]
        public byte NoFollowUp { get; set; }

        public ShexieProvider ShexieProvider { get; set; }

        public ShexiePatient ShexiePatient { get; set; }
    }
}