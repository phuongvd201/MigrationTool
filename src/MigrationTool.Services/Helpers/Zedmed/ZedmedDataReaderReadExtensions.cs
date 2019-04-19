using System.Collections.Generic;
using System.Data.Common;

using MigrationTool.Services.Entities.Zedmed;

namespace MigrationTool.Services.Helpers.Zedmed
{
    internal static class ZedmedDataReaderReadExtensions
    {
        private static string CombineIds(params object[] ids)
        {
            return string.Join("-", ids);
        }

        internal static ZedmedUser GetZedmedUser(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedUser
            {
                Id = reader.GetTrimString(columns["DOCTOR_CODE"]),
                Title = reader.GetTrimString(columns["TITLE"]),
                GivenName = reader.GetString(columns["GIVEN_NAME"]),
                FamilyName = reader.GetString(columns["FAMILY_NAME"]),
                UserName = reader.GetString(columns["USER_NAME"]),
            };
        }

        internal static ZedmedPatient GetZedmedPatient(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedPatient
            {
                Id = reader.GetInt32(columns["PATIENT_ID"]),
                AccounPayerId = reader.GetString(columns["ACCOUNT_PAYER_ID"]),
                PensionStatus = reader.GetString(columns["PENSION_STATUS"]),
                GivenName = reader.GetString(columns["GIVEN_NAME"]),
                FamilyName = reader.GetString(columns["FAMILY_NAME"]),
                PopularName = reader.GetString(columns["POPULAR_NAME"]),
                EmergencyContactName = reader.GetString(columns["EMERG_CONTACT_NAME"]),
                EmergencyContactHomePhone = reader.GetString(columns["EMERG_CONTACT_HOME_PHONE"]),
                EmergencyContactWorkPhone = reader.GetString(columns["EMERG_CONTACT_WORK_PHONE"]),
                EmergencyContactMobilePhone = reader.GetString(columns["EMERG_CONTACT_MOBILE_PHONE"]),
                Gender = reader.GetString(columns["GENDER"]),
                DateOfBirth = reader.GetNullableDateTime(columns["DATE_OF_BIRTH"]),
                HomeAddressLine1 = reader.GetString(columns["HOME_ADDRESS_LINE_1"]),
                HomeAddressLine2 = reader.GetString(columns["HOME_ADDRESS_LINE_2"]),
                HomeSuburbTown = reader.GetString(columns["HOME_SUBURB_TOWN"]),
                HomePostCode = reader.GetString(columns["HOME_POSTCODE"]),
                HomePhone = reader.GetTrimString(columns["HOME_PHONE"]),
                WorkPhone = reader.GetTrimString(columns["WORK_PHONE"]),
                MobilePhone = reader.GetTrimString(columns["MOBILE_PHONE"]),
                EmailAddress = reader.GetString(columns["EMAIL_ADDRESS"]),
                SmokingDetails = reader.GetString(columns["SMOKE_DETAILS"]),
                SmokingStatus = reader.GetString(columns["SMOKE_STATUS"]),
                AlcoholDetails = reader.GetString(columns["ALCOHOL_DETAILS"]),
                MedicareNumber = reader.GetTrimString(columns["MEDICARE_NUMBER"]),
                MedicareNumberExpiryDate = reader.GetNullableDateTime(columns["MEDICARE_NUMBER_EXPIRY"]),
                AllowSms = reader.GetString(columns["ALLOW_SMS"]),
                Occupation = reader.GetString(columns["OCCUPATION"]),
                NokName = reader.GetString(columns["NOK_NAME"]),
                NokHomePhone = reader.GetString(columns["NOK_HOME_PHONE"]),
                NokWorkPhone = reader.GetString(columns["NOK_WORK_PHONE"]),
                NokMobilePhone = reader.GetString(columns["NOK_MOBILE_PHONE"]),
                Alerts = reader.GetString(columns["ALERTS"]),
                MaritalStatus = reader.GetString(columns["MARITAL_STATUS"]),
                VeteranAffairsNumber = reader.GetTrimString(columns["VETERAN_AFFAIRS_NUMBER"]),
                VeteranFileNumberExpiryDate = reader.GetNullableDateTime(columns["VETERAN_FILE_NUMBER_EXPIRY_DATE"]),
                HealthCareCard = reader.GetTrimString(columns["PATIENT_HEALTH_CARE_CARD"]),
                HealthCareCardExpiryDate = reader.GetNullableDateTime(columns["PATIENT_HLTH_CARE_CARD_EX_DATE"]),
                FamilyHistory = reader.GetString(columns["FAMILY_HISTORY"]),
                SocialHistory = reader.GetString(columns["SOCIAL_HISTORY"]),
                PatientNotes = reader.GetString(columns["PATIENT_NOTES"]),
                UsualClinic = reader.GetString(columns["USUAL_CLINIC"]),
                UsualDoctor = reader.GetTrimString(columns["USUAL_DOCTOR"]),
                AccountPayerType = reader.GetString(columns["ACCOUNT_PAYER_TYPE"]),
            };
        }

        internal static ZedmedPatientProblem GetZedmedPatientProblem(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedPatientProblem
            {
                Id = CombineIds(reader.GetInt32(columns["PATIENT_ID"]), reader.GetInt32(columns["PROBLEM_ID"])),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                ProblemText = reader.GetString(columns["PROBLEM_TEXT"]),
                OnsetDate = reader.GetNullableDateTime(columns["ONSET_DATE"]),
            };
        }

        internal static ZedmedAppointment GetZedmedAppointment(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedAppointment
            {
                Id = reader.GetInt32(columns["APPOINTMENT_ID"]),
                PatientId = reader.GetNullableInt32(columns["PATIENT_ID"]),
                DoctorCode = reader.GetTrimString(columns["DOCTOR_CODE"]),
                AppointmentTypeId = reader.GetNullableInt32(columns["APPOINTMENT_TYPE_ID"]),
                BookingTime = reader.GetNullableDateTime(columns["BOOKINGTIME"]),
                StartPoint = reader.GetNullableDateTime(columns["START_POINT"]),
                EndPoint = reader.GetNullableDateTime(columns["END_POINT"]),
                Notes = reader.GetString(columns["NOTES"]),
                StatusCode = reader.GetTrimString(columns["STATUS_CODE"]),
            };
        }

        internal static ZedmedAppointmentType GetZedmedAppointmentTypes(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedAppointmentType
            {
                Id = reader.GetInt32(columns["APPOINTMENT_TYPE_ID"]),
                Description = reader.GetString(columns["DESCRIPTION"]),
                BackgroundColour = reader.GetInt32(columns["BACKGROUND_COLOUR"]),
                Duration = reader.GetDateTime(columns["DURATION"]),
                IsActive = reader.GetString(columns["IS_ACTIVE"]),
            };
        }

        internal static ZedmedContact GetZedmedContact(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedContact
            {
                Id = reader.GetString(columns["REFERRAL_DOCTOR_ID"]),
                Title = reader.GetTrimString(columns["TITLE"]),
                GivenName = reader.GetString(columns["GIVEN_NAME"]),
                FamilyName = reader.GetString(columns["FAMILY_NAME"]),
                WorkAddressLine1 = reader.GetString(columns["WORK_ADDRESS_LINE_1"]),
                WorkAddressLine2 = reader.GetString(columns["WORK_ADDRESS_LINE_2"]),
                HomePhoneNumber = reader.GetString(columns["HOME_PHONE_NUMBER"]),
                MobilePhoneNumber = reader.GetString(columns["MOBILE_PHONE_NUMBER"]),
                WorkPhoneNumber = reader.GetString(columns["WORK_PHONE_NUMBER"]),
                WorkFaxNumber = reader.GetString(columns["WORK_FAX_NUMBER"]),
                Email = reader.GetString(columns["E_MAIL"]),
                ProviderNumber = reader.GetString(columns["PROVIDER_NUMBER"]),
                PostCode = reader.GetString(columns["POSTCODE"]),
                WorkSuburbTown = reader.GetString(columns["WORK_SUBURB_TOWN"]),
            };
        }

        internal static ZedmedReferral GetZedmedReferral(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedReferral
            {
                Id = reader.GetInt32(columns["REFERRAL_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                ReferralDoctortId = reader.GetNullableInt32(columns["REFERRAL_DOCTOR_ID"]),
                ReferralDate = reader.GetNullableDateTime(columns["REFERRAL_DATE"]),
                LetterDate = reader.GetNullableDateTime(columns["LETTER_DATE"]),
                ReferralPeriod = reader.GetInt32(columns["REFERRAL_PERIOD"]),
            };
        }

        internal static ZedmedEncounter GetZedmedEncounter(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedEncounter
            {
                Id = reader.GetInt32(columns["ENCOUNTER_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                DoctorCode = reader.GetTrimString(columns["DOCTOR_CODE"]),
                StartDateTime = reader.GetNullableDateTime(columns["START_DATETIME"]),
                ConvertedData = reader.GetString(columns["CONVERTED_DATA"]),
            };
        }

        internal static ZedmedEncounterNote GetZedmedEncounterNote(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedEncounterNote
            {
                Id = CombineIds(reader.GetNullableInt32(columns["ENCOUNTER_ID"]), reader.GetInt32(columns["SEGMENT_ID"])),
                EncounterId = reader.GetInt32(columns["ENCOUNTER_ID"]),
                SectionNotes = reader.GetString(columns["SECTION_NOTES"]),
            };
        }

        internal static ZedmedAllergy GetZedmedAllergy(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedAllergy
            {
                AllergyDescription = reader.GetString(columns["ALLERGY_DESCRIPTION"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
            };
        }

        internal static ZedmedMeasurement GetZedmedMeasurement(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedMeasurement
            {
                Id = CombineIds(reader.GetNullableDateTime(columns["MEASURE_TIME"]), reader.GetNullableInt32(columns["PATIENT_ID"])),
                PatientId = reader.GetNullableInt32(columns["PATIENT_ID"]),
                MeasureTime = reader.GetNullableDateTime(columns["MEASURE_TIME"]),
                MeasureValue = reader.GetString(columns["MEASURE_VALUE"]),
                MeasureDesc = reader.GetString(columns["MEASURE_DESC"]),
            };
        }

        internal static ZedmedAccountHolder GetZedmedAccountHolder(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedAccountHolder
            {
                Id = reader.GetString(columns["ACCOUNT_PAYER_ID"]),
                HomeAddressLine1 = reader.GetString(columns["HOME_ADDRESS_LINE_1"]),
                HomeAddressLine2 = reader.GetString(columns["HOME_ADDRESS_LINE_2"]),
                DateOfBirth = reader.GetNullableDateTime(columns["DATE_OF_BIRTH"]),
                GivenName = reader.GetString(columns["GIVEN_NAME"]),
                FamilyName = reader.GetString(columns["FAMILY_NAME"]),
                HomePhone = reader.GetString(columns["HOME_PHONE"]),
                MedicareNumberExpiry = reader.GetNullableDateTime(columns["MEDICARE_NUMBER_EXPIRY"]),
                MedicareNumber = reader.GetString(columns["MEDICARE_NUMBER"]),
                HomePostCode = reader.GetString(columns["HOME_POSTCODE"]),
                StatusCode = reader.GetString(columns["STATUS_CODE"]),
                HomeSuburbTown = reader.GetString(columns["HOME_SUBURB_TOWN"]),
                Title = reader.GetTrimString(columns["TITLE"]),
            };
        }

        internal static ZedmedAccountPayer GetZedmedAccountPayer(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedAccountPayer
            {
                Id = reader.GetString(columns["ACCOUNT_PAYER_ID"]),
                AddressLine1 = reader.GetString(columns["ADDRESS_LINE_1"]),
                AddressLine2 = reader.GetString(columns["ADDRESS_LINE_2"]),
                Name = reader.GetString(columns["NAME"]),
                ContactName = reader.GetString(columns["CONTACT_NAME"]),
                FaxNumber = reader.GetString(columns["FAX_NUMBER"]),
                ContactPhone = reader.GetString(columns["CONTACT_PHONE"]),
                PhoneNumber = reader.GetString(columns["PHONE_NUMBER"]),
                SuburbTown = reader.GetString(columns["SUBURB_TOWN"]),
                PostCode = reader.GetTrimString(columns["POSTCODE"]),
            };
        }

        internal static ZedmedInterestedParty GetZedmedInterestedParty(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedInterestedParty
            {
                Id = CombineIds(reader.GetNullableInt32(columns["ADDRESSBOOK_ID"]), reader.GetNullableInt32(columns["PATIENT_ID"])),
                AddressBookId = reader.GetNullableInt32(columns["ADDRESSBOOK_ID"]),
                PatientId = reader.GetNullableInt32(columns["PATIENT_ID"]),
            };
        }

        internal static ZedmedTask GetZedmedTask(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedTask
            {
                Id = reader.GetNullableInt32(columns["ACTION_ID"]),
                PatientId = reader.GetString(columns["PATIENT_ID"]),
                UserName = reader.GetString(columns["USER_NAME"]),
                EnteredBy = reader.GetString(columns["ENTERED_BY"]),
                EnteredDateTime = reader.GetNullableDateTime(columns["ENTERED_DATETIME"]),
                DueDate = reader.GetNullableDateTime(columns["DUE_DATE"]),
                LastPerformedDateTime = reader.GetNullableDateTime(columns["LAST_PERFORMED_DATETIME"]),
                Comments = reader.GetString(columns["COMMENTS"]),
            };
        }

        internal static ZedmedChecklistTemplate GetZedmedChecklistTemplate(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedChecklistTemplate
            {
                Id = reader.GetInt32(columns["FORM_ID"]),
                Name = reader.GetString(columns["FORM_NAME"]),
            };
        }

        internal static ZedmedTemplateControl GetZedmedTemplateControl(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedTemplateControl
            {
                TemplateId = reader.GetInt32(columns["FORM_ID"]),
                ControlId = reader.GetInt32(columns["CONTROL_ID"]),
                ControlTypeId = reader.GetInt32(columns["CONTROL_TYPE_ID"]),
                Value = reader.GetString(columns["VAL"]),
                Property = reader.GetString(columns["PROPERTY"]),
            };
        }

        internal static ZedmedVaccination GetZedmedVaccination(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedVaccination
            {
                ACIRCode = reader.GetString(columns["ACIR_CODE"]),
                Seq = reader.GetInt16(columns["SEQ"]),
                ImmDateTime = reader.GetNullableDateTime(columns["IMM_DATETIME"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                ImmDesc = reader.GetString(columns["IMM_DESC"]),
                ManualImm = reader.GetString(columns["MANUAL_IMM"]),
            };
        }

        internal static ZedmedAttachment GetZedmedAttachment(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedAttachment
            {
                Id = reader.GetInt32(columns["IMAGE_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                FileName = reader.GetString(columns["FILENAME"]),
                SavedDateTime = reader.GetNullableDateTime(columns["SAVED_DATETIME"]),
                Description = reader.GetString(columns["DESCRIPTION"]),
            };
        }

        internal static ZedmedImageDocument GetZedmedImageDocument(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedImageDocument
            {
                Id = reader.GetInt32(columns["IMAGE_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                FileExtension = reader.GetString(columns["FILE_EXTENSION"]),
                SavedDateTime = reader.GetNullableDateTime(columns["SAVED_DATETIME"]),
                ImageDesc = reader.GetString(columns["IMAGE_DESC"]),
            };
        }

        internal static ZedmedLetter GetZedmedLetter(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedLetter
            {
                Id = reader.GetString(columns["OUTBOX_CONTENT_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                DocumentId = reader.GetInt32(columns["DOCUMENT_ID"]),
                PrimaryRecipient = reader.GetNullableInt32(columns["PRIMARY_RECIPIENT"]),
                FromDoctorCode = reader.GetString(columns["FROM_DOCTOR_CODE"]),
                LetterDate = reader.GetNullableDateTime(columns["LETTER_DATE"]),
                SavedDateTime = reader.GetNullableDateTime(columns["SAVED_DATETIME"]),
            };
        }

        internal static ZedmedDrug GetZedmedDrug(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedDrug
            {
                Id = CombineIds(reader.GetString(columns["PRODCODE"]), reader.GetString(columns["FORMCODE"]), reader.GetString(columns["PACKCODE"])),
                ProdCode = reader.GetNullableInt32(columns["PRODCODE"]),
                FormCode = reader.GetNullableInt32(columns["FORMCODE"]),
                PackCode = reader.GetNullableInt32(columns["PACKCODE"]),
                ShortDesc = reader.GetString(columns["SHORT_DESC"]),
                Quantity = reader.GetString(columns["QUANTITY"]),
            };
        }

        internal static ZedmedScript GetZedmedScript(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedScript
            {
                Id = CombineIds(reader.GetString(columns["DRUG_ID"]), reader.GetString(columns["PATIENT_ID"])),
                DateFirstPrescribed = reader.GetNullableDateTime(columns["DATE_FIRST_PRESCRIBED"]),
                DosageFullText = reader.GetString(columns["DOSAGE_FULL_TEXT"]),
                PatientId = reader.GetNullableInt32(columns["PATIENT_ID"]),
                DrugId = CombineIds(reader.GetString(columns["PRODCODE"]), reader.GetString(columns["FORMCODE"]), reader.GetString(columns["PACKCODE"])),
                Notes = reader.GetString(columns["NOTES"]),
                AuthorityNumber = reader.GetString(columns["AUTHORITY_NUMBER"]),
                ScriptDesc = reader.GetString(columns["SCRIPT_DESC"]),
                Repeats = reader.GetString(columns["REPEATS"]),
                DoctorCode = reader.GetTrimString(columns["DOCTOR_CODE"]),
            };
        }

        internal static ZedmedLaboratoryResult GetLaboratoryResults(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedLaboratoryResult
            {
                Id = reader.GetString(columns["RESULT_ID"]),
                PatientId = reader.GetInt32(columns["PATIENT_ID"]),
                DoctorCode = reader.GetTrimString(columns["DOCTOR_CODE"]),
                ReportedDate = reader.GetNullableDateTime(columns["RST_REPORTED_DATE"]),
                SavedDateTime = reader.GetNullableDateTime(columns["SAVED_DATETIME"]),
                CollectedDate = reader.GetNullableDateTime(columns["RST_COLLECTED_DATE"]),
                ReceivedDate = reader.GetNullableDateTime(columns["RST_RECEIVED_BY_LAB_DATE"]),
                Description = reader.GetString(columns["DESCRIPTION"]),
                EdocId = reader.GetString(columns["EDOC_ID"]),
            };
        }

        internal static ZedmedRecall GetZedmedRecall(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedRecall
            {
                CreationDate = reader.GetNullableDateTime(columns["CREATION_DATE"]),
                AttendanceDate = reader.GetNullableDateTime(columns["ATTENDANCE_DATE"]),
                GivenName = reader.GetString(columns["GIVEN_NAME"]),
                FamilyName = reader.GetString(columns["FAMILY_NAME"]),
                Title = reader.GetString(columns["TITLE"]),
                Id = reader.GetString(columns["RECALL_ID"]),
                OnGoing = reader.GetTrimString(columns["ON_GOING"]),
                PatientId = reader.GetString(columns["PATIENT_ID"]),
                HomePhone = reader.GetString(columns["HOME_PHONE"]),
                WorkPhone = reader.GetString(columns["WORK_PHONE"]),
                MobilePhone = reader.GetString(columns["MOBILE_PHONE"]),
                RecallTypeDescription = reader.GetString(columns["RECALL_TYPE_DESCRIPTION"]),
                UsualPeriod = reader.GetTrimString(columns["USUAL_PERIOD"]),
            };
        }

        internal static ZedmedWorkCoverClaim GetWorkCoverClaim(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new ZedmedWorkCoverClaim
            {
                Id = CombineIds(reader.GetString(columns["PATIENT_ID"]), reader.GetString(columns["ACCOUNT_PAYER_ID"])),
                PatientId = reader.GetString(columns["PATIENT_ID"]),
                AccountPayerId = reader.GetString(columns["ACCOUNT_PAYER_ID"]),
                ContactPhone = reader.GetString(columns["CONTACT_PHONE"]),
                Employer = reader.GetString(columns["EMPLOYER"]),
                Name = reader.GetString(columns["NAME"]),
                EntryDate = reader.GetNullableDateTime(columns["ENTRY_DATE"]),
                InsuranceClaimNumber = reader.GetString(columns["INSURANCE_CLAIM_NUMBER"]),
            };
        }
    }
}