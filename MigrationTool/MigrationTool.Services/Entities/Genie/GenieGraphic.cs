using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieGraphic
    {
        public int PatientId { get; set; }

        public string PathName { get; set; }

        public string RealName { get; set; }

        public int Id { get; set; }

        public DateTime? ImageDate { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Description { get; set; }
    }
}