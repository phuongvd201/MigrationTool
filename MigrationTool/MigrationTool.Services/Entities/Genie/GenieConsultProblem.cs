namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieConsultProblem
    {
        public int Id { get; set; }

        public int ConsultId { get; set; }

        public bool IsPrimaryProblem { get; set; }

        public string Problem { get; set; }
    }
}