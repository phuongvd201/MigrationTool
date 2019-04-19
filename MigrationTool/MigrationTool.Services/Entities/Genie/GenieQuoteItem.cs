namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieQuoteItem
    {
        public int Id { get; set; }

        public string ItemNumber { get; set; }

        public string Description { get; set; }

        public short Number { get; set; }

        public float Rebate { get; set; }

        public float Fraction { get; set; }

        public float Fee { get; set; }

        public float Gst { get; set; }

        public float KnownGap { get; set; }

        public float AmaFee { get; set; }

        public string Icd10Code { get; set; }

        public bool AssistantBillable { get; set; }

        public int OpReportId { get; set; }
    }
}