using System;

namespace MigrationTool.Services.Entities.MedicalDirector
{
    internal class MedicalDirectorPatient
    {
        public int Id { get; set; }

        public int? PayerId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string MiddleName { get; set; }

        public string KnownAs { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string StreetLine1 { get; set; }

        public string StreetLine2 { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string HomePhone { get; set; }

        public string WorkPhone { get; set; }

        public string MobilePhone { get; set; }

        public DateTime? DateOfDecease { get; set; }

        public string ChartNumber { get; set; }

        public bool? ReceiveSms { get; set; }

        public string MedicareNo { get; set; }

        public string MedicareIndex { get; set; }

        public DateTime? MedicareExpiryDate { get; set; }

        public string PensionNo { get; set; }

        public DateTime? PensionExpiryDate { get; set; }

        public string DvaNo { get; set; }

        public string InsuranceNo { get; set; }

        public int? InsuranceCompanyId { get; set; }

        public string Occupation { get; set; }

        public string PensionCode { get; set; }

        public int? MaritalStatusCode { get; set; }

        public int? CountryId { get; set; }

        public int? CountryOfBirthId { get; set; }

        public int? LanguageId { get; set; }

        public MedicalDirectorPatientClinical PatientClinical { get; set; }

        public MedicalDirectorNextOfKin NextOfKin { get; set; }

        public MedicalDirectorEmergencyContact EmergencyContact { get; set; }
    }
}