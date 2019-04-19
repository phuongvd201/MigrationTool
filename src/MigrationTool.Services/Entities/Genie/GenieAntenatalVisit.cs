using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieAntenatalVisit
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int PregnancyId { get; set; }

        public DateTime? VisitDate { get; set; }

        public string Gestation { get; set; }

        public int? Fundus { get; set; }

        public float? Weight { get; set; }

        public string Urine { get; set; }

        public string Bp { get; set; }

        public string LiquorVolume { get; set; }

        public string Oedema { get; set; }

        public string Presentation { get; set; }

        public string Station { get; set; }

        public string Fm { get; set; }

        public string Fh { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }
    }
}