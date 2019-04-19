using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "Hosp_Oper")]
    public class ShexieFeeEstimate
    {
        [Column(Name = "Hosplist_Id")]
        public int HospitalId { get; set; }

        [Column(Name = "Operation")]
        public int OperationId { get; set; }

        [Column(Name = "Surgery_Id")]
        public int SurgeryId { get; set; }
    }
}