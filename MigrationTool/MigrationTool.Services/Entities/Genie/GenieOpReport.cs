using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieOpReport
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public int PatientId { get; set; }

        public int AnaesthetistId { get; set; }

        public int AssistantId { get; set; }

        public string ProcedureName { get; set; }

        public int? Side { get; set; }

        public string DoctorName { get; set; }

        public string Hospital { get; set; }

        public DateTime? ProcedureDate { get; set; }

        public TimeSpan? ProcedureTimeFrom { get; set; }

        public TimeSpan? ProcedureTimeTo { get; set; }

        public DateTime? AdmissionDate { get; set; }

        public TimeSpan? AdmissionTime { get; set; }

        public TimeSpan? FastFromTime { get; set; }

        public DateTime? DischargeDate { get; set; }

        public int? InPatientDays { get; set; }

        public string Indication { get; set; }

        public string Category { get; set; }

        public string Magnitude { get; set; }

        public string InfectionRisk { get; set; }

        public string ProcedureType { get; set; }

        public string Anaesthetic { get; set; }

        public string Prosthesis { get; set; }

        public string Finding { get; set; }

        public string Technique { get; set; }

        public string PostOp { get; set; }

        public string AdmissionOutcome { get; set; }

        public DateTime? FollowupDate { get; set; }

        public string FollowupOutcome { get; set; }

        public string AuditSummary { get; set; }

        public string PreopDiagnosis { get; set; }

        public string PostopDiagnosis { get; set; }

        public string QuoteSettings { get; set; }
    }
}