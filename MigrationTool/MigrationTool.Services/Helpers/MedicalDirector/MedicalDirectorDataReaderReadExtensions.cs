using System.Collections.Generic;
using System.Data.Common;

using MigrationTool.Services.Entities.MedicalDirector;

namespace MigrationTool.Services.Helpers.MedicalDirector
{
    internal static class MedicalDirectorDataReaderReadExtensions
    {
        internal static MedicalDirectorResource GetMedicalDirectorResource(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorResource
            {
                ResourceId = reader.GetInt32(columns["RESOURCE_ID"]),
                ResourceType = reader.SafeGetString(columns["RESOURCE_TYPE"]),
                Name = reader.SafeGetString(columns["NAME"])
            };
        }

        internal static MedicalDirectorReferral GetMedicalDirectorReferral(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorReferral
            {
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                ReferralDate = reader.GetNullableDateTime(columns["REFERRING_DATE"]),
                ReferralDoctor = reader.SafeGetString(columns["REFERRING_DOCTOR"]),
            };
        }

        internal static MedicalDirectorAddressBookEntry GetMedicalDirectorAddressBookEntry(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorAddressBookEntry
            {
                AddressBookId = reader.GetInt32(columns["ADDRESS_BOOK_ID"]),
                Title = reader.GetTrimString(columns["TITLE"]),
                FirstName = reader.SafeGetString(columns["FIRST_NAME"]),
                Surname = reader.SafeGetString(columns["SURNAME"]),
                StreetLine1 = reader.SafeGetString(columns["STREET_LINE_1"]),
                StreetLine2 = reader.SafeGetString(columns["STREET_LINE_2"]),
                Category = reader.SafeGetString(columns["CATEGORY"]),
                PostCode = reader.GetTrimString(columns["POSTCODE"]),
                City = reader.SafeGetString(columns["CITY"]),
                Email = reader.SafeGetString(columns["EMAIL"]),
                HomePhone = reader.SafeGetString(columns["PHONE_HOME"]),
                WorkPhone = reader.SafeGetString(columns["PHONE_WORK"]),
                MobilePhone = reader.SafeGetString(columns["PHONE_MOBILE"]),
                Fax = reader.SafeGetString(columns["FAX"]),
                ProviderNo = reader.SafeGetString(columns["PROVIDER_NO"]),
                HPINo = reader.SafeGetString(columns["HPI_NO"]),
                HealthLink = reader.SafeGetString(columns["HEALTHLINK_EDI"]),
                ReferralPeriod = reader.GetNullableInt32(columns["REFERRAL_PERIOD"]),
            };
        }

        internal static MedicalDirectorPatient GetMedicalDirectorPatient(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorPatient
            {
                Id = reader.GetInt32(columns["PATIENT_ID"]),
                Title = reader.SafeGetString(columns["TITLE"]),
                FirstName = reader.SafeGetString(columns["FIRST_NAME"]),
                Surname = reader.SafeGetString(columns["SURNAME"]),
                MiddleName = reader.SafeGetString(columns["MIDDLE_NAME"]),
                KnownAs = reader.SafeGetString(columns["KNOWN_AS"]),
                DateOfBirth = reader.GetNullableDateTime(columns["DOB"]),
                Gender = reader.GetTrimString(columns["GENDER_CODE"]),
                StreetLine1 = reader.SafeGetString(columns["STREET_LINE_1"]),
                StreetLine2 = reader.SafeGetString(columns["STREET_LINE_2"]),
                City = reader.SafeGetString(columns["CITY"]),
                PostCode = reader.SafeGetString(columns["POSTCODE"]),
                HomePhone = reader.SafeGetString(columns["PHONE_HOME"]),
                WorkPhone = reader.SafeGetString(columns["PHONE_WORK"]),
                MobilePhone = reader.SafeGetString(columns["PHONE_MOBILE"]),
                DateOfDecease = reader.GetNullableDateTime(columns["DECEASED_DATE"]),
                ChartNumber = reader.SafeGetString(columns["CHART_NO"]),
                ReceiveSms = reader.GetNullableBoolean(columns["RECEIVE_SMS"]),
                MedicareNo = reader.SafeGetString(columns["MEDICARE_NO"]),
                MedicareIndex = reader.GetTrimString(columns["MEDICARE_INDEX"]),
                MedicareExpiryDate = reader.GetNullableDateTime(columns["MEDICARE_EXPIRY_DATE"]),
                PensionNo = reader.SafeGetString(columns["PENSION_NO"]),
                PensionCode = reader.GetTrimString(columns["PENSION_CODE"]),
                PensionExpiryDate = reader.GetNullableDateTime(columns["PENSION_EXPIRY_DATE"]),
                DvaNo = reader.SafeGetString(columns["DVA_NO"]),
                InsuranceNo = reader.SafeGetString(columns["INSURANCE_NO"]),
                InsuranceCompanyId = reader.GetNullableInt32(columns["INSURANCE_COMPANY_ID"]),
                Occupation = reader.SafeGetString(columns["DESCRIPTION"]),
                MaritalStatusCode = reader.GetNullableInt32(columns["MARITAL_STATUS_CODE"]),
                CountryId = reader.GetNullableInt32(columns["COUNTRY_OF_BIRTH_ID"]),
                CountryOfBirthId = reader.GetNullableInt32(columns["COUNTRY_OF_BIRTH_ID2"]),
                LanguageId = reader.GetNullableInt32(columns["LANGUAGE_ID"]),
                PayerId = reader.GetNullableInt32(columns["PAYER_ID"]),
            };
        }

        internal static MedicalDirectorPatientClinical GetMedicalDirectorPatientClinical(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorPatientClinical
            {
                Id = reader.GetInt32(columns["PATIENT_CLINICAL_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                Warnings = reader.SafeGetString(columns["WARNINGS"]),
                FamilyHistory = reader.SafeGetString(columns["FAMILY_HISTORY"]),
                Notes = reader.SafeGetString(columns["NOTES"]),
                Social = reader.SafeGetString(columns["SOCIAL"]),
                Smoker = reader.GetTrimString(columns["SMOKER"]),
                SmokingEx = reader.SafeGetString(columns["SMOKING_EX"]),
                AlcoholEx = reader.SafeGetString(columns["ALCOHOL_EX"]),
            };
        }

        internal static MedicalDirectorNextOfKin GetMedicalDirectorNextOfKin(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorNextOfKin
            {
                Id = reader.GetInt32(columns["NEXT_OF_KIN_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                Title = reader.SafeGetString(columns["TITLE"]),
                FirstName = reader.SafeGetString(columns["FIRST_NAME"]),
                Surname = reader.SafeGetString(columns["SURNAME"]),
                HomePhone = reader.SafeGetString(columns["PHONE_HOME"]),
                MobilePhone = reader.SafeGetString(columns["PHONE_MOBILE"]),
                WorkPhone = reader.SafeGetString(columns["PHONE_WORK"]),
                StreetLine1 = reader.SafeGetString(columns["STREET_LINE_1"]),
                StreetLine2 = reader.SafeGetString(columns["STREET_LINE_2"]),
            };
        }

        internal static MedicalDirectorEmergencyContact GetMedicalDirectorEmergencyContact(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorEmergencyContact
            {
                Id = reader.GetInt32(columns["ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                Title = reader.SafeGetString(columns["TITLE"]),
                FirstName = reader.SafeGetString(columns["FIRST_NAME"]),
                Surname = reader.SafeGetString(columns["SURNAME"]),
                HomePhone = reader.SafeGetString(columns["PHONE_HOME"]),
                MobilePhone = reader.SafeGetString(columns["PHONE_MOBILE"]),
                WorkPhone = reader.SafeGetString(columns["PHONE_WORK"]),
            };
        }

        internal static MedicalDirectorAhiaTrade GetMedicalDirectorAhiaTrade(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorAhiaTrade
            {
                Id = reader.GetInt32(columns["ID"]),
                Trading = reader.SafeGetString(columns["TRADING"]),
            };
        }

        internal static MedicalDirectorCountry GetMedicalDirectorCountry(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorCountry
            {
                Id = reader.GetInt32(columns["COUNTRY_ID"]),
                Name = reader.SafeGetString(columns["NAME"]),
            };
        }

        internal static MedicalDirectorLanguage GetMedicalDirectorLanguage(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new MedicalDirectorLanguage
            {
                Id = reader.GetInt32(columns["LANGUAGE_ID"]),
                Language = reader.SafeGetString(columns["LANGUAGE"]),
            };
        }
    }
}