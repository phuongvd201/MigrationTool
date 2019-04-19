namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedEncounterNote
    {
        public string Id { get; set; }

        public int EncounterId { get; set; }

        public string SectionNotes { get; set; }

        public ZedmedEncounter Encounter { get; set; }
    }
}