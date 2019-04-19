using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieComplication
    {
        public int Id { get; set; }

        public int ProcedureId { get; set; }

        public DateTime? ComplicationDate { get; set; }

        public string ComplicationDetails { get; set; }
    }
}