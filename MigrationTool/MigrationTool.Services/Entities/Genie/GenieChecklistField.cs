namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieChecklistField
    {
        public int Id { get; set; }

        public int ChecklistId { get; set; }

        public int ChecklistFieldId { get; set; }

        public string Type { get; set; }

        public int Position { get; set; }

        public string Label { get; set; }

        public string FieldData { get; set; }
    }
}