namespace MigrationTool.Services.Entities.MedicalDirector
{
    internal class MedicalDirectorAddressBookEntry
    {
        public int AddressBookId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string StreetLine1 { get; set; }

        public string StreetLine2 { get; set; }

        public string Category { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string HomePhone { get; set; }

        public string WorkPhone { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        public string ProviderNo { get; set; }

        public string HPINo { get; set; }

        public string HealthLink { get; set; }

        public int? ReferralPeriod { get; set; }
    }
}