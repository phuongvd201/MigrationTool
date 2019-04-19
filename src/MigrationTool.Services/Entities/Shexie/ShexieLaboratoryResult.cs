using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "PathoTest")]
    public class ShexieLaboratoryResult
    {
        [Column(Name = "testID")]
        public int Id { get; set; }

        [Column(Name = "runID")]
        public int? RunId { get; set; }

        [Column(Name = "patient_no")]
        public int? PatientId { get; set; }

        [Column(Name = "test_name")]
        public string ResultName { get; set; }

        [Column(Name = "Results")]
        public string Results { get; set; }

        [Column(Name = "normal")]
        public string Normal { get; set; }

        [Column(Name = "patho_prov")]
        public string Provider { get; set; }

        [Column(Name = "date_collected")]
        public DateTime? DateCollected { get; set; }

        [Column(Name = "Compressed")]
        public byte Compressed { get; set; }

        public ShexiePatient Patient { get; set; }

        public ShexieLaboratoryResultType ResultType { get; set; }
    }
}