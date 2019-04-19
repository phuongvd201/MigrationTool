namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieDrug
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Form { get; set; }

        public string Strength { get; set; }

        public string Quantity { get; set; }

        public string Code { get; set; }

        public int? MimsProdCode { get; set; }

        public int? MimsFormCode { get; set; }

        public int? MimsPackCode { get; set; }
    }
}