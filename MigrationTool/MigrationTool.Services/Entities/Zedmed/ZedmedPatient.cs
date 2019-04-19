using System;

namespace MigrationTool.Services.Entities.Zedmed
{
    internal class ZedmedPatient
    {
        public int Id { get; set; }

        public string AccounPayerId { get; set; }

        public string PensionStatus { get; set; }

        public string UsualClinic { get; set; }

        public string UsualDoctor { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string PopularName { get; set; }

        public string EmergencyContactName { get; set; }

        public string EmergencyContactHomePhone { get; set; }

        public string EmergencyContactWorkPhone { get; set; }

        public string EmergencyContactMobilePhone { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string HomeAddressLine1 { get; set; }

        public string HomeAddressLine2 { get; set; }

        public string HomeSuburbTown { get; set; }

        public string HomePostCode { get; set; }

        public string HomePhone { get; set; }

        public string WorkPhone { get; set; }

        public string MobilePhone { get; set; }

        public string EmailAddress { get; set; }

        public string SmokingDetails { get; set; }

        public string SmokingStatus { get; set; }

        public string AlcoholDetails { get; set; }

        public string MedicareNumber { get; set; }

        public DateTime? MedicareNumberExpiryDate { get; set; }

        public string MaritalStatus { get; set; }

        public string AllowSms { get; set; }

        public string VeteranAffairsNumber { get; set; }

        public DateTime? VeteranFileNumberExpiryDate { get; set; }

        public string HealthCareCard { get; set; }

        public DateTime? HealthCareCardExpiryDate { get; set; }

        public string Occupation { get; set; }

        public string NokName { get; set; }

        public string NokHomePhone { get; set; }

        public string NokWorkPhone { get; set; }

        public string NokMobilePhone { get; set; }

        public string Alerts { get; set; }

        public string FamilyHistory { get; set; }

        public string SocialHistory { get; set; }

        public string PatientNotes { get; set; }

        public string AccountPayerType { get; set; }
    }
}