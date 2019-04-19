using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "patient")]
    public class ShexiePatient : ISuburbInfo
    {
        [Column(Name = "patientno")]
        public int Id { get; set; }

        [Column(Name = "sname")]
        public string LastName { get; set; }

        [Column(Name = "MName")]
        public string MiddleName { get; set; }

        [Column(Name = "cname")]
        public string FirstName { get; set; }

        [Column(Name = "street1")]
        public string AddressLine1 { get; set; }

        [Column(Name = "street2")]
        public string AddressLine2 { get; set; }

        [Column(Name = "hphone")]
        public string HomePhone { get; set; }

        [Column(Name = "bphone")]
        public string WorkPhone { get; set; }

        [Column(Name = "mobile")]
        public string MobilePhone { get; set; }

        [Column(Name = "dob")]
        public DateTime? DateOfBirth { get; set; }

        [Column(Name = "medicare")]
        public string MedicareNum { get; set; }

        [Column(Name = "expiry")]
        public DateTime? MedicareExpiryDate { get; set; }

        [Column(Name = "repat")]
        public string VetAffNo { get; set; }

        [Column(Name = "pension")]
        public string PensionNo { get; set; }

        [Column(Name = "HCC")]
        public string HCCNo { get; set; }

        [Column(Name = "HCCExpiry")]
        public DateTime? HCCExpiry { get; set; }

        [Column(Name = "occupation")]
        public string Occupation { get; set; }

        [Column(Name = "sex")]
        public string Gender { get; set; }

        [Column(Name = "No_Correspondence")]
        public byte? NoCorrespondence { get; set; }

        [Column(Name = "Guardian")]
        public int? GuardianId { get; set; }

        [Column(Name = "Mother")]
        public int? MotherId { get; set; }

        [Column(Name = "Father")]
        public int? FatherId { get; set; }

        [Column(Name = "Other")]
        public int? OtherId { get; set; }

        [Column(Name = "suburbid")]
        public int? SuburbId { get; set; }

        [Column(Name = "patientno")]
        public int? PatientSecondaryId { get; set; }

        [Column(Name = "cardno")]
        public byte? CardNo { get; set; }

        public int? EmergencyPersonId { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string Suburb { get; set; }

        public string PostCode { get; set; }

        public ShexieKin EmergencyPerson { get; set; }

        public ShexieContact Guardian { get; set; }

        public ShexiePatientSecondary PatientSecondary { get; set; }

        public ShexieCompany Company { get; set; }
    }
}