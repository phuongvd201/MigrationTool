using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieChecklist
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int PRCDRE_Id_Fk { get; set; }

        public DateTime? DateCreated { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }
    }
}