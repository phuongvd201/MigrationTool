using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

using MigrationTool.Services.Entities.MedicalDirector;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Helpers.MedicalDirector;
using MigrationTool.Services.Interfaces.MedicalDirector;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations.MedicalDirector
{
    internal class MedicalDirectorRepository : IMedicalDirectorRepository, IMedicalDirectorConnectionTestService
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static readonly DbProviderFactory MedicalDirectorDbProvider = DbProviderFactories.GetFactory("System.Data.SqlClient");

        private readonly IMedicalDirectorSettingsService localMedicalDirectorSettingsService;

        public MedicalDirectorRepository(IMedicalDirectorSettingsService medicalDirectorSettingsService)
        {
            localMedicalDirectorSettingsService = medicalDirectorSettingsService;
        }

        private static readonly string[] AllFields = { "*" };

        private static readonly string[] UserFields =
        {
            "RESOURCE_ID",
            "RESOURCE_TYPE",
            "NAME",
        };

        private static readonly string[] PatientFields =
        {
            "CM_PATIENT.PATIENT_ID",
            "CM_PATIENT.PAYER_ID",
            "CM_PATIENT.TITLE",
            "CM_PATIENT.FIRST_NAME",
            "CM_PATIENT.SURNAME",
            "CM_PATIENT.MIDDLE_NAME",
            "CM_PATIENT.KNOWN_AS",
            "CM_PATIENT.DOB",
            "CM_PATIENT.GENDER_CODE",
            "CM_PATIENT.STREET_LINE_1",
            "CM_PATIENT.STREET_LINE_2",
            "CM_PATIENT.CITY",
            "CM_PATIENT.POSTCODE",
            "CM_PATIENT.PHONE_HOME",
            "CM_PATIENT.PHONE_WORK",
            "CM_PATIENT.PHONE_MOBILE",
            "CM_PATIENT.DECEASED_DATE",
            "CM_PATIENT.CHART_NO",
            "CM_PATIENT.RECEIVE_SMS",
            "CM_PATIENT.MEDICARE_NO",
            "CM_PATIENT.MEDICARE_INDEX",
            "CM_PATIENT.MEDICARE_EXPIRY_DATE",
            "CM_PATIENT.PENSION_NO",
            "CM_PATIENT.PENSION_CODE",
            "CM_PATIENT.PENSION_EXPIRY_DATE",
            "CM_PATIENT.DVA_NO",
            "CM_PATIENT.INSURANCE_NO",
            "CM_PATIENT.INSURANCE_COMPANY_ID",
            "CM_PATIENT.INSURANCE_TYPE",
            "CM_OCCUPATION.DESCRIPTION",
            "CM_PATIENT.MARITAL_STATUS_CODE",
            "CM_PATIENT.COUNTRY_OF_BIRTH_ID",
            "CM_PATIENT.COUNTRY_OF_BIRTH_ID2",
            "CM_PATIENT_LANGUAGE.LANGUAGE_ID",
        };

        private static readonly string[] PatientClinicalFields =
        {
            "PATIENT_CLINICAL_ID",
            "PATIENT_ID",
            "WARNINGS",
            "FAMILY_HISTORY",
            "NOTES",
            "SOCIAL",
            "SMOKER",
            "SMOKING_EX",
            "ALCOHOL_EX",
        };

        private static readonly string[] NextOfKinFields =
        {
            "CM_NEXT_OF_KIN.NEXT_OF_KIN_ID",
            "CM_NEXT_OF_KIN.PATIENT_ID",
            "CM_PATIENT.TITLE",
            "CM_PATIENT.FIRST_NAME",
            "CM_PATIENT.SURNAME",
            "CM_PATIENT.PHONE_HOME",
            "CM_PATIENT.PHONE_MOBILE",
            "CM_PATIENT.PHONE_WORK",
            "CM_PATIENT.STREET_LINE_1",
            "CM_PATIENT.STREET_LINE_2",
        };

        private static readonly string[] EmergencyContactFields =
        {
            "CM_EMERGENCY_CONTACT.ID",
            "CM_EMERGENCY_CONTACT.PATIENT_ID",
            "CM_PATIENT.TITLE",
            "CM_PATIENT.FIRST_NAME",
            "CM_PATIENT.SURNAME",
            "CM_PATIENT.PHONE_HOME",
            "CM_PATIENT.PHONE_MOBILE",
            "CM_PATIENT.PHONE_WORK",
        };

        private static readonly string[] ReferralFields =
        {
            "PATIENT_ID",
            "REFERRING_DOCTOR",
            "REFERRING_DATE",
        };

        public IEnumerable<MedicalDirectorResource> GetResources()
        {
            return QuerySql(localMedicalDirectorSettingsService.HcnConnectionString, UserFields, "CM_RESOURCE", null, MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorResource);
        }

        public IEnumerable<MedicalDirectorReferral> GetReferrals()
        {
            var where = "REFERRING_DOCTOR is not null";
            return QuerySql(localMedicalDirectorSettingsService.HcnConnectionString, ReferralFields, "CM_PATIENT", where, MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorReferral);
        }

        public IEnumerable<MedicalDirectorAddressBookEntry> GetAddressBookEntries()
        {
            return QuerySql(localMedicalDirectorSettingsService.HcnConnectionString, AllFields, "CM_ADDRESS_BOOK", "COMPANY = 0", MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorAddressBookEntry);
        }

        public IEnumerable<MedicalDirectorPatient> GetPatients()
        {
            var from = @"CM_PATIENT LEFT JOIN CM_OCCUPATION on CM_PATIENT.OCCUPATION_ID = CM_OCCUPATION.OCCUPATION_ID 
                            LEFT JOIN CM_PATIENT_LANGUAGE ON CM_PATIENT.PATIENT_ID = CM_PATIENT_LANGUAGE.PATIENT_ID AND CM_PATIENT_LANGUAGE.PREFERRED = 1";
            return QuerySql(
                localMedicalDirectorSettingsService.HcnConnectionString,
                PatientFields,
                from,
                "CM_PATIENT.PATIENT_ID <> 0",
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorPatient)
                .GroupJoin(
                    GetPatientClinicals().ToDictionary(x => x.PatientId),
                    x => x.Id,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.PatientClinical = o.Select(x => x.Value).FirstOrDefault() ?? new MedicalDirectorPatientClinical();
                        return i;
                    })
                .GroupJoin(
                    GetNextOfKins().ToDictionary(x => x.PatientId),
                    x => x.Id,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.NextOfKin = o.Select(x => x.Value).FirstOrDefault() ?? new MedicalDirectorNextOfKin();
                        return i;
                    })
                .GroupJoin(
                    GetEmergencyContacts().ToDictionary(x => x.PatientId),
                    x => x.Id,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.EmergencyContact = o.Select(x => x.Value).FirstOrDefault() ?? new MedicalDirectorEmergencyContact();
                        return i;
                    });
        }

        public IEnumerable<MedicalDirectorPatientClinical> GetPatientClinicals()
        {
            return QuerySql(
                localMedicalDirectorSettingsService.HcnConnectionString,
                PatientClinicalFields,
                "MD_PATIENT_CLINICAL",
                null,
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorPatientClinical);
        }

        public IEnumerable<MedicalDirectorNextOfKin> GetNextOfKins()
        {
            return QuerySql(
                localMedicalDirectorSettingsService.HcnConnectionString,
                NextOfKinFields,
                "CM_NEXT_OF_KIN inner join CM_PATIENT ON CM_NEXT_OF_KIN.RELATION_ID = CM_PATIENT.PATIENT_ID",
                null,
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorNextOfKin);
        }

        public IEnumerable<MedicalDirectorEmergencyContact> GetEmergencyContacts()
        {
            return QuerySql(
                localMedicalDirectorSettingsService.HcnConnectionString,
                EmergencyContactFields,
                "CM_EMERGENCY_CONTACT inner join CM_PATIENT ON CM_EMERGENCY_CONTACT.EMERGENCY_CONTACT_PATIENT_ID = CM_PATIENT.PATIENT_ID ",
                null,
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorEmergencyContact);
        }

        public IEnumerable<MedicalDirectorAhiaTrade> GetAhiaTrades()
        {
            return QuerySql(
                localMedicalDirectorSettingsService.ContentConnectionString,
                AllFields,
                "MDDATA_AHIATRADE",
                null,
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorAhiaTrade);
        }

        public IEnumerable<MedicalDirectorCountry> GetCountries()
        {
            return QuerySql(
                localMedicalDirectorSettingsService.ContentConnectionString,
                AllFields,
                "CM_COUNTRY",
                "DELETED = 0",
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorCountry);
        }

        public IEnumerable<MedicalDirectorLanguage> GetLanguages()
        {
            return QuerySql(
                localMedicalDirectorSettingsService.ContentConnectionString,
                AllFields,
                "CM_LANGUAGE",
                "DELETED = 0",
                MedicalDirectorDataReaderReadExtensions.GetMedicalDirectorLanguage);
        }

        private static IEnumerable<TEntity> QuerySql<TEntity>(string connectionString, string[] fields, string table, string where, Func<DbDataReader, Dictionary<string, int>, TEntity> create)
        {
            return DbHelper.QuerySql(MedicalDirectorDbProvider, connectionString, fields, table, where, create, Log);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(localMedicalDirectorSettingsService.HcnConnectionString))
                {
                    connection.Open();
                }
                using (var connection = new SqlConnection(localMedicalDirectorSettingsService.ContentConnectionString))
                {
                    connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Connection test failed.", ex);
                return false;
            }
        }
    }
}