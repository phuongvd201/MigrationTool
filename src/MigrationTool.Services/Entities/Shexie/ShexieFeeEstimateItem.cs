using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "operations")]
    public class ShexieFeeEstimateItem
    {
        [Column(Name = "id")]
        public int Id { get; set; }

        [Column(Name = "ICD_Code")]
        public string Icd10Code { get; set; }

        [Column(Name = "MBS_Code")]
        public string ItemNumber { get; set; }

        [Column(Name = "opdesc")]
        public string Description { get; set; }
    }
}