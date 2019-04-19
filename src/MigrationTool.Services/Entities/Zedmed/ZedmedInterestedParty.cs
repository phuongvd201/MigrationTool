namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedInterestedParty
    {
        public string Id { get; set; }

        public int? PatientId { get; set; }

        public int? AddressBookId { get; set; }
    }
}