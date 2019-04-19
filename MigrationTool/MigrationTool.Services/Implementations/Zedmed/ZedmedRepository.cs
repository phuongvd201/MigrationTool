using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

using FirebirdSql.Data.FirebirdClient;

using MigrationTool.Services.Entities.Zedmed;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Helpers.Zedmed;
using MigrationTool.Services.Interfaces.Zedmed;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations.Zedmed
{
    internal class ZedmedRepository : IZedmedRepository, IZedmedConnectionTestService
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static readonly DbProviderFactory ZedmedDbProvider = FirebirdClientFactory.Instance;

        public IZedmedSettingsService ZedmedSettingsService { get; set; }

        private static readonly string[] AllFields = { "*" };

        private static readonly string[] UserFields =
        {
            "TREATING_DOCTORS.DOCTOR_CODE",
            "TREATING_DOCTORS.TITLE",
            "TREATING_DOCTORS.GIVEN_NAME",
            "TREATING_DOCTORS.FAMILY_NAME",
            "STAFF.USER_NAME"
        };

        private static readonly string[] ContactFields =
        {
            "RD.REFERRAL_DOCTOR_ID",
            "RD.TITLE",
            "RD.GIVEN_NAME",
            "RD.FAMILY_NAME",
            "RDL.WORK_ADDRESS_LINE_1",
            "RDL.WORK_ADDRESS_LINE_2",
            "RD.HOME_PHONE_NUMBER",
            "RD.MOBILE_PHONE_NUMBER",
            "RD.E_MAIL",
            "RDL.POSTCODE",
            "RDL.WORK_SUBURB_TOWN",
            "RDL.PROVIDER_NUMBER",
            "RDL.WORK_FAX_NUMBER",
            "RDL.WORK_PHONE_NUMBER",
        };

        private static readonly string[] AccountHolderFields =
        {
            "DISTINCT BT.ACCOUNT_PAYER_ID",
            "PT.HOME_ADDRESS_LINE_1",
            "PT.HOME_ADDRESS_LINE_2",
            "PT.DATE_OF_BIRTH",
            "PT.GIVEN_NAME",
            "PT.FAMILY_NAME",
            "PT.HOME_PHONE",
            "PT.MEDICARE_NUMBER_EXPIRY",
            "PT.MEDICARE_NUMBER",
            "PT.HOME_POSTCODE",
            "PT.STATUS_CODE",
            "PT.HOME_SUBURB_TOWN",
            "PT.TITLE",
        };

        private static readonly string[] AccountPayerFields =
        {
            "ACCOUNT_PAYER_ID",
            "ADDRESS_LINE_1",
            "ADDRESS_LINE_2",
            "NAME",
            "CONTACT_NAME",
            "FAX_NUMBER",
            "CONTACT_PHONE",
            "PHONE_NUMBER",
            "SUBURB_TOWN",
            "POSTCODE",
        };

        private static readonly string[] PatientFields =
        {
            "PATIENTS.PATIENT_ID",
            "BILL_TO.ACCOUNT_PAYER_ID",
            "PENSION_STATUS",
            "GIVEN_NAME",
            "FAMILY_NAME",
            "POPULAR_NAME",
            "EMERG_CONTACT_NAME",
            "EMERG_CONTACT_HOME_PHONE",
            "EMERG_CONTACT_WORK_PHONE",
            "EMERG_CONTACT_MOBILE_PHONE",
            "GENDER",
            "DATE_OF_BIRTH",
            "HOME_ADDRESS_LINE_1",
            "HOME_ADDRESS_LINE_2",
            "HOME_SUBURB_TOWN",
            "HOME_POSTCODE",
            "HOME_PHONE",
            "WORK_PHONE",
            "MOBILE_PHONE",
            "EMAIL_ADDRESS",
            "SMOKE_DETAILS",
            "SMOKE_STATUS",
            "ALCOHOL_DETAILS",
            "MEDICARE_NUMBER",
            "MEDICARE_NUMBER_EXPIRY",
            "ALLOW_SMS",
            "OCCUPATION",
            "NOK_NAME",
            "NOK_HOME_PHONE",
            "NOK_WORK_PHONE",
            "NOK_MOBILE_PHONE",
            "ALERTS",
            "MARITAL_STATUS",
            "VETERAN_AFFAIRS_NUMBER",
            "VETERAN_FILE_NUMBER_EXPIRY_DATE",
            "PATIENT_HEALTH_CARE_CARD",
            "PATIENT_HLTH_CARE_CARD_EX_DATE",
            "FAMILY_HISTORY",
            "SOCIAL_HISTORY",
            "PATIENT_NOTES",
            "USUAL_CLINIC",
            "USUAL_DOCTOR",
            "ACCOUNT_PAYER_TYPE",
        };

        private static readonly string[] PatientProblemFields =
        {
            "PROBLEM_ID",
            "PATIENT_ID",
            "PROBLEM_TEXT",
            "ONSET_DATE",
        };

        private static readonly string[] AppointmentFields =
        {
            "APPOINTMENT_ID",
            "DOCTOR_CODE",
            "PATIENT_ID",
            "APPOINTMENT_TYPE_ID",
            "NOTES",
            "BOOKINGTIME",
            "START_POINT",
            "END_POINT",
            "STATUS_CODE"
        };

        private static readonly string[] AllergyFields =
        {
            "ALLERGY_DESCRIPTION",
            "PATIENT_ID",
        };

        private static readonly string[] TemplateControlFields =
        {
            "FORM_ID",
            "CONTROL_ID",
            "VAL",
            "CONTROL_TYPE_ID",
            "PROPERTY",
        };

        private static readonly string[] EncounterFields =
        {
            "CRS_ENCOUNTER.ENCOUNTER_ID",
            "CRS_ENCOUNTER.PATIENT_ID",
            "CRS_ENCOUNTER.DOCTOR_CODE",
            "CRS_ENCOUNTER.START_DATETIME",
            "CRS_ENCOUNTER.CONVERTED_DATA",
        };

        private static readonly string[] EncounterNoteFields =
        {
            "CRS_ENC_SEGMENT_NOTES.ENCOUNTER_ID",
            "CRS_ENC_SEGMENT_NOTES.SEGMENT_ID",
            "CRS_ENC_SEGMENT_NOTES.SECTION_NOTES",
        };

        private static readonly string[] ChecklistTemplateFields =
        {
            "CRS_ENC_TEMPLATE_FORM.FORM_ID",
            "CRS_ENC_TEMPLATE_FORM.FORM_NAME",
        };

        private static readonly string[] MeasurementFields =
        {
            "CRS_ENC_SEGMENT_MEASURE.PATIENT_ID",
            "CRS_ENC_SEGMENT_MEASURE.MEASURE_TYPE_ID",
            "CRS_ENC_SEGMENT_MEASURE.MEASURE_VALUE",
            "CRS_ENC_SEGMENT_MEASURE.MEASURE_TIME",
            "CRS_MEASURE_TYPE.MEASURE_DESC",
        };

        private static readonly string[] VaccinationFields =
        {
            "CRS_ENC_SEGMENT_IMM.MANUAL_IMM",
            "CRS_ENC_SEGMENT_IMM.IMM_DATETIME",
            "CRS_ENC_SEGMENT_IMM.SEQ",
            "CRS_IMM_TYPE.IMM_DESC",
            "CRS_IMM_TYPE.ACIR_CODE",
            "CRS_ENCOUNTER.PATIENT_ID"
        };

        private static readonly string[] AttachmentFields =
        {
            "CRS_ATTACHMENT.IMAGE_ID",
            "CRS_ATTACHMENT.PATIENT_ID",
            "CRS_ATTACHMENT.FILENAME",
            "CRS_ATTACHMENT.DESCRIPTION",
            "CRS_ATTACHMENT.SAVED_DATETIME",
        };

        private static readonly string[] LetterFields =
        {
            "OUTBOX_CONTENT.OUTBOX_CONTENT_ID",
            "OUTBOX_CONTENT.DOCUMENT_ID",
            "OUTBOX_CONTENT.PATIENT_ID",
            "OUTBOX_CONTENT.FROM_DOCTOR_CODE",
            "OUTBOX_CONTENT.PRIMARY_RECIPIENT",
            "CRS_PATIENT_DOCUMENT.LETTER_DATE",
            "CRS_PATIENT_DOCUMENT.SAVED_DATETIME"
        };

        private static readonly string[] InterestedPartiesFields =
        {
            "PATIENT_PROVIDERS.PATIENT_ID",
            "PATIENT_PROVIDERS.ADDRESSBOOK_ID",
        };

        private static readonly string[] DrugFields =
        {
            "PRODCODE",
            "FORMCODE",
            "PACKCODE",
            "SHORT_DESC",
            "QUANTITY",
        };

        private static readonly string[] ScriptFields =
        {
            "CPD.PRODCODE",
            "CPD.FORMCODE",
            "CPD.PACKCODE",
            "CPD.SCRIPT_DESC",
            "CPD.PATIENT_ID",
            "CPD.DRUG_ID",
            "CPD.DOSAGE_FULL_TEXT",
            "CPD.REPEATS",
            "CPD.DATE_FIRST_PRESCRIBED",
            "CPD.NOTES",
            "CPD.AUTHORITY_NUMBER",
            "CE.DOCTOR_CODE",
        };

        private static readonly string[] LaboratoryResultsFields =
        {
            "CRR.RESULT_ID",
            "CRR.PATIENT_ID",
            "CE.DOCTOR_CODE",
            "CE.RST_REPORTED_DATE",
            "CRR.SAVED_DATETIME",
            "CE.RST_COLLECTED_DATE",
            "CE.RST_RECEIVED_BY_LAB_DATE",
            "CRR.DESCRIPTION",
            "CE.EDOC_ID",
        };

        private static readonly string[] ImageDocumentFields =
        {
            "CRS_IMAGE2.IMAGE_ID",
            "CRS_IMAGE_DOCUMENT.PATIENT_ID",
            "CRS_IMAGE_DOCUMENT.IMAGE_DESC",
            "CRS_IMAGE_DOCUMENT.SAVED_DATETIME",
            "IMAGE.FILE_EXTENSION",
        };

        private static readonly string[] RecallFields =
        {
            "RECALLS.PATIENT_ID",
            "RECALLS.RECALL_ID",
            "RECALLS.DOCTOR_CODE",
            "RECALLS.CREATION_DATE",
            "RECALLS.ATTENDANCE_DATE",
            "RECALLS.ON_GOING",
            "RECALL_TYPES.RECALL_TYPE_DESCRIPTION",
            "RECALL_TYPES.USUAL_PERIOD",
            "RECALL_TYPES.IS_ACTIVE",
            "PATIENTS.HOME_PHONE",
            "PATIENTS.WORK_PHONE",
            "PATIENTS.MOBILE_PHONE",
            "TREATING_DOCTORS.TITLE",
            "TREATING_DOCTORS.GIVEN_NAME",
            "TREATING_DOCTORS.FAMILY_NAME",
        };

        private static readonly string[] WorkCoverFields =
        {
            "ACCIDENTS.PATIENT_ID",
            "ACCIDENTS.INSURANCE_CLAIM_NUMBER",
            "ACCIDENTS.ACCOUNT_PAYER_ID",
            "ACCIDENTS.EMPLOYER",
            "ACCIDENTS.ENTRY_DATE",
            "ACCOUNT_PAYERS.CONTACT_PHONE",
            "ACCOUNT_PAYERS.NAME",
        };

        public bool TestConnection()
        {
            try
            {
                using (var superplusConnection = new FbConnection(ZedmedSettingsService.SuperplusConnectionString))
                {
                    superplusConnection.Open();
                }

                using (var clinplusConnection = new FbConnection(ZedmedSettingsService.ClinplusConnectionString))
                {
                    clinplusConnection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Connection test failed.", ex);
                return false;
            }
        }

        public IEnumerable<ZedmedUser> GetUsers()
        {
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                UserFields,
                "TREATING_DOCTORS left join STAFF on TREATING_DOCTORS.DOCTOR_CODE = STAFF.DOCTOR_CODE",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedUser);
        }

        public IEnumerable<ZedmedPatient> GetPatients()
        {
            var where = "(BILL_TO.CURRENT_ACCOUNT = 'Y') OR (BILL_TO.CURRENT_ACCOUNT IS NULL)";
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                PatientFields,
                "PATIENTS LEFT JOIN BILL_TO ON PATIENTS.PATIENT_ID = BILL_TO.PATIENT_ID",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedPatient);
        }

        public IEnumerable<ZedmedPatientProblem> GetPatientProblems()
        {
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                PatientProblemFields,
                "CRS_PATIENT_PROBLEM",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedPatientProblem);
        }

        public IEnumerable<ZedmedAppointment> GetAppointments(DateTime? start)
        {
            var where = start.HasValue ? string.Format("(BOOKINGTIME >= '{0:yyyy-MM-dd}')", start.Value) : null;
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                AppointmentFields,
                "UNIFIED_APPOINTMENTS",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedAppointment);
        }

        public IEnumerable<ZedmedReferral> GetReferrals()
        {
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                AllFields,
                "REFERRALS",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedReferral);
        }

        public IEnumerable<ZedmedAppointmentType> GetAppointmentTypes()
        {
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                AllFields,
                "APPOINTMENT_TYPES",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedAppointmentTypes);
        }

        public IEnumerable<ZedmedContact> GetContacts()
        {
            var where = "RD.IS_ACTIVE = 'Y' AND RDL.LOCATION_ID = "
                        + "(SELECT MAX(LOCATION_ID) FROM REFERRAL_DOCTOR_LOCATIONS WHERE REFERRAL_DOCTOR_ID = RD.REFERRAL_DOCTOR_ID AND MAIN_LOCATION = 'Y' AND IS_ACTIVE = 'Y')";
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                ContactFields,
                "REFERRAL_DOCTORS RD Inner Join REFERRAL_DOCTOR_LOCATIONS RDL On RD.REFERRAL_DOCTOR_ID = RDL.REFERRAL_DOCTOR_ID",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedContact);
        }

        public IEnumerable<ZedmedEncounter> GetEncounters(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("(CRS_ENCOUNTER.CREATION_DATETIME >= '{0:yyyy-MM-dd}')", start.Value)
                : null;
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                EncounterFields,
                "CRS_ENCOUNTER",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedEncounter);
        }

        public IEnumerable<ZedmedEncounterNote> GetEncounterNotes()
        {
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                EncounterNoteFields,
                "CRS_ENC_SEGMENT_NOTES",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedEncounterNote);
        }

        public IEnumerable<ZedmedAllergy> GetAllergies()
        {
            var where = "ALLERGY_DESCRIPTION is not null and ALLERGY_DESCRIPTION <> ''";
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                AllergyFields,
                "CRS_PATIENT_ALLERGY",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedAllergy);
        }

        public IEnumerable<ZedmedMeasurement> GetMeasurements(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("(MEASURE_TIME >= '{0:yyyy-MM-dd}')", start.Value)
                : null;
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                MeasurementFields,
                "CRS_ENC_SEGMENT_MEASURE inner join CRS_MEASURE_TYPE on CRS_ENC_SEGMENT_MEASURE.MEASURE_TYPE_ID=CRS_MEASURE_TYPE.MEASURE_TYPE_ID",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedMeasurement);
        }

        public IEnumerable<ZedmedTemplateControl> GetTemplateControls()
        {
            var where = "CONTROL_TYPE_ID NOT IN (1,0,8)";
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                TemplateControlFields,
                "CRS_ENC_TEMPLATE_DESIGN",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedTemplateControl);
        }

        public IEnumerable<ZedmedTask> GetTasks(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((ENTERED_DATETIME >= '{0:yyyy-MM-dd}') or (LAST_PERFORMED_DATETIME >= '{0:yyyyMMddHHmm}'))", start.Value)
                : null;
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                AllFields,
                "CRS_ACTIONLIST",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedTask);
        }

        public IEnumerable<ZedmedVaccination> GetVaccinations(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((IMM_DATETIME >= '{0:yyyy-MM-dd}'))", start.Value)
                : null;
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                VaccinationFields,
                "CRS_ENCOUNTER inner join  CRS_ENC_SEGMENT_IMM on CRS_ENCOUNTER.ENCOUNTER_ID = CRS_ENC_SEGMENT_IMM.ENCOUNTER_ID " +
                "left join CRS_IMM_TYPE on CRS_ENC_SEGMENT_IMM.IMM_TYPE_ID = CRS_IMM_TYPE.IMM_TYPE_ID",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedVaccination);
        }

        public IEnumerable<ZedmedAttachment> GetAttachments(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("(CRS_ATTACHMENT.SAVED_DATETIME >= '{0:yyyy-MM-dd}')", start.Value)
                : null;
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                AttachmentFields,
                "CRS_ATTACHMENT",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedAttachment);
        }

        public IEnumerable<ZedmedChecklistTemplate> GetChecklistTemplates()
        {
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                ChecklistTemplateFields,
                "CRS_ENC_TEMPLATE_FORM",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedChecklistTemplate);
        }

        public IEnumerable<ZedmedAccountHolder> GetAccountHolders()
        {
            var where = "BT.ACCOUNT_PAYER_TYPE = 'P' AND CURRENT_ACCOUNT= 'Y'";
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                AccountHolderFields,
                "PATIENTS PT inner join BILL_TO BT on PT.PATIENT_ID = BT.ACCOUNT_PAYER_ID",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedAccountHolder);
        }

        public IEnumerable<ZedmedAccountPayer> GetAccountPayers()
        {
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                AccountPayerFields,
                "ACCOUNT_PAYERS",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedAccountPayer);
        }

        public IEnumerable<ZedmedLetter> GetLetters()
        {
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                LetterFields,
                "OUTBOX_CONTENT Inner Join CRS_PATIENT_DOCUMENT On OUTBOX_CONTENT.PATIENT_ID = CRS_PATIENT_DOCUMENT.PATIENT_ID And OUTBOX_CONTENT.DOCUMENT_ID = CRS_PATIENT_DOCUMENT.DOCUMENT_ID",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedLetter);
        }

        public IEnumerable<ZedmedInterestedParty> GetInterestedParties()
        {
            var where = "PATIENTS.GP_ID is not null";
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                InterestedPartiesFields,
                "PATIENT_PROVIDERS UNION SELECT PATIENTS.PATIENT_ID, PATIENTS.GP_ID as ADDRESSBOOK_ID FROM PATIENTS",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedInterestedParty);
        }

        public IEnumerable<ZedmedDrug> GetDrugs()
        {
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                DrugFields,
                "CRS_PATIENT_DRUG",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedDrug);
        }

        public IEnumerable<ZedmedScript> GetScripts()
        {
            var where = "CPD.SCRIPT_NUMBER is not null and CE.DOCTOR_CODE is not null and CE.DOCTOR_CODE <> '' ";
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                ScriptFields,
                "CRS_PATIENT_DRUG CPD inner join CRS_ENC_SEGMENT_DRUG CESD on CPD.SCRIPT_NUMBER  = CESD.SCRIPT_NUMBER and CPD.DRUG_ID  = CESD.DRUG_ID " +
                "inner join CRS_ENCOUNTER CE on CE.ENCOUNTER_ID = CESD.ENCOUNTER_ID",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedScript);
        }

        public IEnumerable<ZedmedLaboratoryResult> GetLaboratoryResults()
        {
            var where = "CRR.PATIENT_ID is not null";
            var laboratoryResult = QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                LaboratoryResultsFields,
                "CRS_REF_RESULT CRR inner join CRS_EDOCUMENT CE on CRR.EDOC_ID = CE.EDOC_ID",
                where,
                ZedmedDataReaderReadExtensions.GetLaboratoryResults);
            return laboratoryResult;
        }

        public IEnumerable<ZedmedImageDocument> GetImageDocuments()
        {
            return QuerySql(
                ZedmedSettingsService.ClinplusConnectionString,
                ImageDocumentFields,
                "CRS_IMAGE_DOCUMENT INNER JOIN CRS_IMAGE2 ON CRS_IMAGE_DOCUMENT.IMAGE_DOC_ID = CRS_IMAGE2.IMAGE_DOC_ID " +
                "INNER JOIN IMAGE ON CRS_IMAGE2.IMAGE_ID = IMAGE.IMAGE_ID",
                null,
                ZedmedDataReaderReadExtensions.GetZedmedImageDocument);
        }

        public IEnumerable<ZedmedRecall> GetRecalls()
        {
            var where = "RECALL_TYPES.IS_ACTIVE = 'Y'";
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                RecallFields,
                "RECALLS INNER JOIN RECALL_TYPES ON RECALLS.RECALL_TYPE_ID = RECALL_TYPES.RECALL_TYPE_ID " +
                "INNER JOIN PATIENTS ON RECALLS.PATIENT_ID = PATIENTS.PATIENT_ID " +
                "INNER JOIN TREATING_DOCTORS ON RECALLS.DOCTOR_CODE = TREATING_DOCTORS.DOCTOR_CODE",
                where,
                ZedmedDataReaderReadExtensions.GetZedmedRecall);
        }

        public IEnumerable<ZedmedWorkCoverClaim> GetWorkCoverClaims()
        {
            return QuerySql(
                ZedmedSettingsService.SuperplusConnectionString,
                WorkCoverFields,
                "ACCIDENTS inner join ACCOUNT_PAYERS on ACCIDENTS.ACCOUNT_PAYER_ID = ACCOUNT_PAYERS.ACCOUNT_PAYER_ID",
                null,
                ZedmedDataReaderReadExtensions.GetWorkCoverClaim);
        }

        private static IEnumerable<TEntity> QuerySql<TEntity>(string connectionString, string[] fields, string table, string where, Func<DbDataReader, Dictionary<string, int>, TEntity> create)
        {
            return DbHelper.QuerySql(ZedmedDbProvider, connectionString,  fields, table, where, create, Log);
        }
    }
}