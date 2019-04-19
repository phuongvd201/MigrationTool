using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "history")]
    public class ShexiePatientHistory
    {
        [Column(Name = "cnt")]
        public int Id { get; set; }

        [Column(Name = "patient")]
        public int PatientId { get; set; }

        [Column(Name = "date")]
        public DateTime? Date { get; set; }

        [Column(Name = "purpose")]
        public string Note { get; set; }

        [Column(Name = "Formatted")]
        public byte Formatted { get; set; }
    }
}