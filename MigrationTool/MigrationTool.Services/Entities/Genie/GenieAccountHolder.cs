using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieAccountHolder
    {
        public int Id { get; set; }

        public string Organisation { get; set; }

        public bool Individual { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string FullName { get; set; }

        public string Fax { get; set; }

        public string Mobile { get; set; }

        public string HomePhone { get; set; }

        public string MedicareNum { get; set; }

        public int MedicareRefNum { get; set; }

        public DateTime? MedicareExpiry { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}