namespace MigrationTool.Services.Entities.MedicalDirector
{
    internal class MedicalDirectorNextOfKin
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string HomePhone { get; set; }

        public string MobilePhone { get; set; }

        public string WorkPhone { get; set; }

        public string StreetLine1 { get; set; }

        public string StreetLine2 { get; set; }
    }
}