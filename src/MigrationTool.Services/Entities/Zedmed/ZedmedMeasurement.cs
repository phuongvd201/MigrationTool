using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedMeasurement
    {
        public string Id { get; set; }

        public int? PatientId { get; set; }

        public DateTime? MeasureTime { get; set; }

        public string MeasureDesc { get; set; }

        public string MeasureValue { get; set; }
    }
}