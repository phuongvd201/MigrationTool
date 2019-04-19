namespace MigrationTool.Services.Entities.MedicalDirector
{
    internal class MedicalDirectorPatientClinical
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Warnings { get; set; }

        public string FamilyHistory { get; set; }

        public string Social { get; set; }

        public string Notes { get; set; }

        public string Smoker { get; set; }

        public string SmokingEx { get; set; }

        public string AlcoholEx { get; set; }
    }
}