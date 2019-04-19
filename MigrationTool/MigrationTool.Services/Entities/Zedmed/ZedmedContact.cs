namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedContact
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string WorkAddressLine1 { get; set; }

        public string WorkAddressLine2 { get; set; }

        public string HomePhoneNumber { get; set; }

        public string MobilePhoneNumber { get; set; }

        public string WorkPhoneNumber { get; set; }

        public string WorkFaxNumber { get; set; }

        public string Email { get; set; }

        public string ProviderNumber { get; set; }

        public string PostCode { get; set; }

        public string WorkSuburbTown { get; set; }
    }
}