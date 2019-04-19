using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GeniePatient
    {
        public int Id { get; set; }

        public int AccountHolderId { get; set; }

        public int UsualGpId { get; set; }

        public string UsualProvider { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string KnownAs { get; set; }

        public DateTime? Dob { get; set; }

        public DateTime? Dod { get; set; }

        public bool Deceased { get; set; }

        public string ChartNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string Suburb { get; set; }

        public string PostCode { get; set; }

        public string Sex { get; set; }

        public string HomePhone { get; set; }

        public string SmokingInfo { get; set; }

        public string AlcoholInfo { get; set; }

        public string Title { get; set; }

        public string WorkPhone { get; set; }

        public string MedicareNum { get; set; }

        public int MedicareRefNum { get; set; }

        public string State { get; set; }

        public string EmailAddress { get; set; }

        public string MiddleName { get; set; }

        public string CountryOfBirth { get; set; }

        public string Language { get; set; }

        public string AddressLine2 { get; set; }

        public string MaritalStatus { get; set; }

        public string DvaNumber { get; set; }

        public string DvaDisability { get; set; }

        public string DvaCardColour { get; set; }

        public string MobilePhone { get; set; }

        public DateTime? MedicareExpiry { get; set; }

        public string Country { get; set; }

        public string PartnerName { get; set; }

        public string MaidenName { get; set; }

        public string AccountType { get; set; }

        public bool DoNotSendSms { get; set; }

        public string HealthFundName { get; set; }

        public string HealthFundNumber { get; set; }

        public string HccPensionNumber { get; set; }

        public DateTime? HccPensionExpiry { get; set; }

        public string Scratchpad { get; set; }

        public int? SmokingFreq { get; set; }

        public bool Alcohol { get; set; }

        public string NokName { get; set; }

        public string NokPhone { get; set; }

        public string Memo { get; set; }

        public string ExternalId { get; set; }
    }
}