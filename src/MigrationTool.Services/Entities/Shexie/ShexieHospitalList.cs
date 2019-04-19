using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "hosplist")]
    public class ShexieHospitalList
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "surg_date")]
        public DateTime? SurgeryDate { get; set; }

        [Column(Name = "surg_time")]
        public DateTime? SurgeryTime { get; set; }

        [Column(Name = "SurgEndTime")]
        public DateTime? SurgeryEndTime { get; set; }

        [Column(Name = "admission_date")]
        public DateTime? Admission { get; set; }

        [Column(Name = "discharge_date")]
        public DateTime? DischargeDate { get; set; }
    }
}