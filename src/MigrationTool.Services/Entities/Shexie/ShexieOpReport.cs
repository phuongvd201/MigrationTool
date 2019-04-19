using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "surgery")]
    public class ShexieOpReport
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "patient")]
        public int? PatientId { get; set; }

        [Column(Name = "surgeon")]
        public int? SurgeonId { get; set; }

        [Column(Name = "sdate")]
        public DateTime? Date { get; set; }

        [Column(Name = "findings")]
        public string Finding { get; set; }

        [Column(Name = "description")]
        public string Name { get; set; }

        [Column(Name = "details")]
        public string Details { get; set; }

        [Column(Name = "Anaesthetist")]
        public int? AnaesthetistId { get; set; }

        [Column(Name = "Assistant")]
        public int? AssistantId { get; set; }

        [Column(Name = "Provider")]
        public string Provider { get; set; }

        public IEnumerable<ShexieFeeEstimateItem> FeeEstimateItems { get; set; }

        public ShexieHospitalList ShexieHospitalList { get; set; }
    }
}