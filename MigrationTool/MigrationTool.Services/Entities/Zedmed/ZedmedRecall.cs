using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedRecall
    {
        public string Id { get; set; }

        public string PatientId { get; set; }

        public string RecallTypeDescription { get; set; }

        public string UsualPeriod { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? AttendanceDate { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Title { get; set; }

        public string HomePhone { get; set; }

        public string WorkPhone { get; set; }

        public string MobilePhone { get; set; }

        public string OnGoing { get; set; }
    }
}