using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedVaccination
    {
        public string ImmDesc { get; set; }

        public int Seq { get; set; }

        public int PatientId { get; set; }

        public DateTime? ImmDateTime { get; set; }

        public string ManualImm { get; set; }

        public string ACIRCode { get; set; }
    }
}