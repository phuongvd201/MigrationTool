using System;
using System.Collections.Generic;
using System.Linq;

using MigrationTool.Services.Entities.MedicalDirector;

using Siberia.Migration.Entities;

namespace MigrationTool.Services.Helpers.MedicalDirector
{
    internal static class MedicalDirectorMigrationHelper
    {
        private static readonly string[] MedicalDirectorReceptionistResourceType =
        {
            "RECEPTIONIST",
        };

        private static readonly Dictionary<string, string> SalutationMedicalDirectorToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Dr", "Dr" },
            { "Prof", "Prof" },
            { "A Prof", "A Prof" },
            { "Mr", "Mr" },
            { "Mrs", "Mrs" },
            { "Ms", "Ms" },
            { "Miss", "Miss" },
            { "Master", "Master" },
            { "Soldier", "Soldier" },
            { "Private", "Private" },
            { "Lance Corporal", "Lance Corporal" },
            { "Corporal", "Corporal" },
            { "Sergeant", "Sergeant" },
            { "Staff Sergeant", "Staff Sergeant" },
            { "Warrant Officer", "Warrant Officer" },
            { "Lieutenant", "Lieutenant" },
            { "Captain", "Captain" },
            { "Major", "Major" },
            { "Lieutenant Colonel", "Lieutenant Colonel" },
            { "Colonel", "Colonel" },
            { "Sister", "Sister" },
            { "Father", "Father" },
            { "DR.", "Dr" },
        };

        private static readonly Dictionary<string, string> MaritalStatusMedicalDirectorToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "0", null },
            { "1", "Never married" },
            { "2", "Married" },
            { "3", "Widowed" },
            { "4", "Divorced" },
            { "5", "De facto" },
            { "6", "Separated" },
        };

        private static readonly Dictionary<string, string> SmokingStatusMedicalDirectorToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "N", "Never smoker" },
            { "Y", "Current every day smoker" },
            { "X", "Former smoker" },
        };

        private static readonly Dictionary<string, string> AccountTypeMedicalDirectorToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "N", "Private" },
            { "P", "Health Fund" },
            { "R", "DVA" },
            { "L", "DVA" },
        };

        private static readonly Dictionary<string, string> HealthFundCodeMedicalDirectorToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "MBF AUSTRALIA LTD", "MBF" },
            { "MEDIBANK PRIVATE LTD", "MPL" },
            { "ACA HEALTH BENEFITS FUND", "ACA" },
            { "AUSTRALIAN HEALTH MANAGEMENT", "AHM" },
            { "AUSTRALIAN UNITY HEALTH LIMITED", "AUH" },
            { "CBHS HEALTH FUND Ltd", "CBH" },
            { "CENTRAL WEST HEALTH", "HHB" },
            { "CY HEALTH COVER", "CYF" },
            { "DEFENCE HEALTH", "DHF" },
            { "GMF HEALTH", "HHB" },
            { "GRAND UNITED CORPORATE", "GUH" },
            { "HEALTH CARE INSURANCE LTD", "HCI" },
            { "HEALTH-PARTNERS", "SPS" },
            { "HIF", "HIF" },
            { "PEOPLECARE HEALTH INSURANCE", "LHM" },
            { "NAVY HEALTH LTD", "NHB" },
            { "PHOENIX HEALTH FUND LIMITED", "PHF" },
            { "POLICE HEALTH", "POL" },
            { "QUEENSLAND COUNTRY HEALTH LTD", "QCH" },
            { "RESERVE BANK HEALTH SOCIETY LIMITED", "RBH" },
            { "TEACHERS FEDERATION HEALTH", "TFH" },
            { "TEACHERS' UNION HEALTH", "QTU" },
            { "TRANSPORT HEALTH", "TFS" },
            { "HEALTH INSURANCE FUND of W.A. INC", "HIF" },
            { "ST LUKE'S MEDICAL & HOSPITAL BENEFITS ASSOCIATION LTD", "SLM" },
            { "GMHBA LTD", "GMH" },
            { "LATROBE HEALTH SERVICES INC", "LHS" },
            { "HBF HEALTH FUNDS INC", "HBF" },
            { "HBA", "HBA" },
            { "MUTUAL COMMUNITY", "MCL" },
            { "ANZ HEALTH INSURANCE", "ANZ" },
            { "BUPA AUSTRALIA HEALTH PTY LTD", "BUP" },
            { "HOSPITALS CONTRIBUTION FUND OF AUSTRALIA LTD, THE", "HCF" },
            { "NIB HEALTH FUNDS LTD", "NIB" },
            { "THE DOCTORS' HEALTH FUND LIMITED", "AMA" },
            { "WESTFUND", "WFD" },
        };

        internal static MigrationUser ToMigrationUser(this MedicalDirectorResource medicalDirectorResource)
        {
            var parsedName = medicalDirectorResource.Name.ParseName(SalutationMedicalDirectorToSiberia.Keys.ToArray());

            return new MigrationUser
            {
                ExternalId = medicalDirectorResource.ResourceId.ToString(),
                Salutation = parsedName.Salutation,
                FirstName = parsedName.FirstName,
                Surname = parsedName.LastName,
                IsReceptionist = MedicalDirectorReceptionistResourceType.Contains(medicalDirectorResource.ResourceType, StringComparer.InvariantCultureIgnoreCase),
            };
        }

        internal static MigrationAccountHolder ToMigrationAccountHolder(this MedicalDirectorPatient medicalDirectorPatient, Dictionary<int, int[]> payerIds)
        {
            return new MigrationAccountHolder
            {
                ExternalId = medicalDirectorPatient.Id.ToString(),
                AddressLine1 = medicalDirectorPatient.StreetLine1,
                AddressLine2 = medicalDirectorPatient.StreetLine2,
                DateOfBirth = medicalDirectorPatient.DateOfBirth,
                FirstName = medicalDirectorPatient.FirstName,
                HomePhone = medicalDirectorPatient.HomePhone,
                Individual = true,
                MedicareExpiryDate = medicalDirectorPatient.MedicareExpiryDate,
                MedicareNum = medicalDirectorPatient.MedicareNo,
                MedicareRefNum = medicalDirectorPatient.MedicareIndex.GetNullableInt().GetValueOrDefault(),
                Organisation = medicalDirectorPatient.Occupation,
                PostCode = medicalDirectorPatient.PostCode,
                LastName = medicalDirectorPatient.Surname,
                Salutation = medicalDirectorPatient.Title,
                PatientExternalIds = payerIds.GetValueOrDefault(medicalDirectorPatient.Id, new int[] { }).Select(x => x.ToString()).ToArray(),
            };
        }

        internal static MigrationReferral ToMigrationReferral(this MedicalDirectorReferral medicalDirectorReferral, Dictionary<string, MedicalDirectorAddressBookEntry> contacts)
        {
            var referral = new MigrationReferral
            {
                PatientExternalId = medicalDirectorReferral.PatientId.ToString(),
                ReferralDate = medicalDirectorReferral.ReferralDate,
            };

            var addressBook = contacts.GetValueOrNull(medicalDirectorReferral.ReferralDoctor);
            if (addressBook != null)
            {
                referral.ExternalId = addressBook.AddressBookId.ToString();
                referral.Duration = addressBook.ReferralPeriod.GetValueOrDefault();
                referral.ContactExternalId = addressBook.AddressBookId.ToString();
            }
            return referral;
        }

        internal static MigrationContact ToMigrationContact(this MedicalDirectorAddressBookEntry medicalDirectorAddressBookEntry)
        {
            return new MigrationContact
            {
                ExternalId = medicalDirectorAddressBookEntry.AddressBookId.ToString(),
                Salutation = medicalDirectorAddressBookEntry.Title.Translate(SalutationMedicalDirectorToSiberia),
                FirstName = medicalDirectorAddressBookEntry.FirstName,
                LastName = medicalDirectorAddressBookEntry.Surname,
                AddressLine1 = medicalDirectorAddressBookEntry.StreetLine1,
                AddressLine2 = medicalDirectorAddressBookEntry.StreetLine2,
                Category = medicalDirectorAddressBookEntry.Category,
                PostCode = medicalDirectorAddressBookEntry.PostCode,
                Suburb = medicalDirectorAddressBookEntry.City,
                Email = medicalDirectorAddressBookEntry.Email,
                HomePhone = medicalDirectorAddressBookEntry.HomePhone,
                WorkPhone = medicalDirectorAddressBookEntry.WorkPhone,
                MobilePhone = medicalDirectorAddressBookEntry.MobilePhone,
                Fax = medicalDirectorAddressBookEntry.Fax,
                ProviderNo = medicalDirectorAddressBookEntry.ProviderNo,
                HPI = medicalDirectorAddressBookEntry.HPINo,
                HealthLink = medicalDirectorAddressBookEntry.HealthLink,
            };
        }

        internal static MigrationPatient ToMigrationPatient(
            this MedicalDirectorPatient medicalDirectorPatient,
            Dictionary<int, string> medicalDirectorAhiaTrades,
            Dictionary<int, string> medicalDirectorCountries,
            Dictionary<int, string> medicalDirectorLanguages)
        {
            var nextOfKin = medicalDirectorPatient.NextOfKin;
            var emergencyContact = medicalDirectorPatient.EmergencyContact;
            var patientClinical = medicalDirectorPatient.PatientClinical;

            return new MigrationPatient
            {
                ExternalId = medicalDirectorPatient.Id.ToString(),
                AccountHolderExternalId = medicalDirectorPatient.PayerId.ToString(),
                AccountType = medicalDirectorPatient.PensionCode.Translate(AccountTypeMedicalDirectorToSiberia),
                Salutation = medicalDirectorPatient.Title.Translate(SalutationMedicalDirectorToSiberia),
                FirstName = medicalDirectorPatient.FirstName,
                LastName = medicalDirectorPatient.Surname,
                MiddleName = medicalDirectorPatient.MiddleName,
                AddressLine1 = medicalDirectorPatient.StreetLine1,
                AddressLine2 = medicalDirectorPatient.StreetLine2,
                Suburb = medicalDirectorPatient.City,
                PostCode = medicalDirectorPatient.PostCode,
                Gender = medicalDirectorPatient.Gender.EqualsIgnoreCase("M"),
                DateOfBirth = medicalDirectorPatient.DateOfBirth,
                HomePhone = medicalDirectorPatient.HomePhone,
                WorkPhone = medicalDirectorPatient.WorkPhone,
                MobilePhone = medicalDirectorPatient.MobilePhone,
                KnownAs = medicalDirectorPatient.KnownAs,
                Occupation = medicalDirectorPatient.Occupation,
                DateOfDecease = medicalDirectorPatient.DateOfDecease,
                ChartNumber = medicalDirectorPatient.ChartNumber,
                DoNotSendSms = !medicalDirectorPatient.ReceiveSms,
                MedicareNum = medicalDirectorPatient.MedicareNo,
                MedicareRefNum = medicalDirectorPatient.MedicareIndex.GetNullableInt().GetValueOrDefault(),
                MedicareExpiryDate = medicalDirectorPatient.MedicareExpiryDate,
                HccPensionNumber = medicalDirectorPatient.PensionNo,
                HccPensionExpiry = medicalDirectorPatient.PensionExpiryDate.AsDisplayDateString(),
                DvaNumber = medicalDirectorPatient.DvaNo,
                HealthFundNumber = medicalDirectorPatient.InsuranceNo,
                MaritalStatus = medicalDirectorPatient.MaritalStatusCode.ToString().Translate(MaritalStatusMedicalDirectorToSiberia),
                Country = medicalDirectorCountries.GetValueOrNull(medicalDirectorPatient.CountryId),
                CountryOfBirth = medicalDirectorCountries.GetValueOrNull(medicalDirectorPatient.CountryOfBirthId),
                Language = medicalDirectorLanguages.GetValueOrNull(medicalDirectorPatient.LanguageId),
                Notes = patientClinical.Notes,
                PreviousIssues = string.Join(Environment.NewLine, patientClinical.FamilyHistory, patientClinical.Social),
                SmokingStatus = patientClinical.Smoker.Translate(SmokingStatusMedicalDirectorToSiberia),
                SmokingNotes = patientClinical.SmokingEx,
                DrinkingNotes = patientClinical.AlcoholEx,
                NextOfKinName = ToFullName(nextOfKin.Title, nextOfKin.FirstName, nextOfKin.Surname),
                NextOfKinContactPhone = GetActivePhone(nextOfKin.MobilePhone, nextOfKin.HomePhone, nextOfKin.WorkPhone),
                NextOfKinAddress = nextOfKin.StreetLine1.NullIfEmpty() ?? nextOfKin.StreetLine2,
                EmergencyPersonName = ToFullName(emergencyContact.Title, emergencyContact.FirstName, emergencyContact.Surname),
                EmergencyPersonPhone = GetActivePhone(emergencyContact.MobilePhone, emergencyContact.HomePhone, emergencyContact.WorkPhone),
                HealthFundCode = medicalDirectorAhiaTrades.GetValueOrNull(medicalDirectorPatient.InsuranceCompanyId).Translate(HealthFundCodeMedicalDirectorToSiberia),
                HealthFundName = medicalDirectorAhiaTrades.GetValueOrNull(medicalDirectorPatient.InsuranceCompanyId),
            };
        }

        public static string ToFullName(string salutation, string firstName, string lastName)
        {
            var names = new[] { salutation, firstName, lastName };

            return string.Join(" ", names.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string GetActivePhone(string mobilePhone, string homePhone, string workPhone)
        {
            return new[] { mobilePhone, homePhone, workPhone }.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }
    }
}