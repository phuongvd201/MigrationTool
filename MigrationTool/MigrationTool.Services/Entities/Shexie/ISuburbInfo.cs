namespace MigrationTool.Services.Entities.Shexie
{
    internal interface ISuburbInfo
    {
        int? SuburbId { get; set; }

        string Suburb { get; set; }

        string Country { get; set; }

        string State { get; set; }

        string PostCode { get; set; }
    }
}