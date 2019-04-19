using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "statistics")]
    public class ShexieStatistic
    {
        [Column(Name = "patient")]
        public int PatientId { get; set; }

        [Column(Name = "stat")]
        public int AnalysisId { get; set; }

        [Column(Name = "type")]
        public int Type { get; set; }

        public ShexieAnalysis Analysis { get; set; }
    }
}