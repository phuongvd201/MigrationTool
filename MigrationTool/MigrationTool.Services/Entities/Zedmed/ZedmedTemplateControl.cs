namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedTemplateControl
    {
        public int TemplateId { get; set; }

        public int ControlId { get; set; }

        public int ControlTypeId { get; set; }

        public string Value { get; set; }

        public string Property { get; set; }
    }
}