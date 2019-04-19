namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedDrug
    {
        public string Id { get; set; }

        public string ShortDesc { get; set; }

        public string Quantity { get; set; }

        public int? ProdCode { get; set; }

        public int? FormCode { get; set; }

        public int? PackCode { get; set; }
    }
}