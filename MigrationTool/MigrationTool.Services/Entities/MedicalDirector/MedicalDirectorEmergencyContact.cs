namespace MigrationTool.Services.Entities.MedicalDirector
{
    internal class MedicalDirectorEmergencyContact
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string HomePhone { get; set; }

        public string MobilePhone { get; set; }

        public string WorkPhone { get; set; }
    }
}