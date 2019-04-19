using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedAccountHolder
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string HomePhone { get; set; }

        public string MedicareNumber { get; set; }

        public DateTime? MedicareNumberExpiry { get; set; }

        public string HomeAddressLine1 { get; set; }

        public string HomeAddressLine2 { get; set; }

        public string HomeSuburbTown { get; set; }

        public string StatusCode { get; set; }

        public string HomePostCode { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}