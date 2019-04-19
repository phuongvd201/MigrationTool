using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedAppointmentType
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int BackgroundColour { get; set; }

        public DateTime Duration { get; set; }

        public string IsActive { get; set; }
    }
}