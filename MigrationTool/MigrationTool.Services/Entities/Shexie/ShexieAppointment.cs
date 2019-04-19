using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "appointments")]
    public class ShexieAppointment
    {
        [Column(Name = "cnt")]
        public int Id { get; set; }

        [Column(Name = "comment")]
        public string Description { get; set; }

        [Column(Name = "appdate")]
        public DateTime? StartDate { get; set; }

        [Column(Name = "apptime")]
        public DateTime? StartTime { get; set; }

        [Column(Name = "Date_Made")]
        public DateTime? CreationDate { get; set; }

        [Column(Name = "patient")]
        public int PatientId { get; set; }

        [Column(Name = "app_type")]
        public int AppointmentTypeId { get; set; }

        [Column(Name = "status")]
        public byte Status { get; set; }

        [Column(Name = "provider")]
        public string Provider { get; set; }

        public ShexieAppointmentType AppointmentType { get; set; }
    }
}