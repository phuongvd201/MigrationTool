using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedAppointment
    {
        public int Id { get; set; }

        public string Notes { get; set; }

        public DateTime? StartPoint { get; set; }

        public DateTime? EndPoint { get; set; }

        public string DoctorCode { get; set; }

        public int? PatientId { get; set; }

        public DateTime? BookingTime { get; set; }

        public int? AppointmentTypeId { get; set; }

        public string StatusCode { get; set; }
    }
}