using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieDownloadedResult
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Addressee { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? ReportDate { get; set; }

        public DateTime? ImportDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? CollectionDate { get; set; }

        public string LabRef { get; set; }

        public string Test { get; set; }

        public string Result { get; set; }

        public string DocumentName { get; set; }

        public string NormalOrAbnormal { get; set; }

        public string ExternalPatientId { get; set; }
    }
}