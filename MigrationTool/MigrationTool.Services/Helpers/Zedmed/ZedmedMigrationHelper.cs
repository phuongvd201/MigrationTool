using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using MigrationTool.Services.Entities.Zedmed;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Helpers.Zedmed
{
    internal static class ZedmedMigrationHelper
    {
        public const string ChecklistHtmlDivTagPattern = @"<div[^>]*>((?!<div).)*({0})( - )(.*?)<\/( )*div>";
        private const string ChecklistFieldNameWithValuePattern = @"({0})( )([^\(]*?);";
        private const string AppointmentCancelledByPatient = "1";
        private const int ZedmedPatientIdMaxDigit = 10;

        public static class ZedmedDocumentDirectory
        {
            public const string PatientDocument = "CRS_PATIENT_DOCUMENT/DOCUMENT_BLOB/";
            public const string ResultDocument = "CRS_EDOCUMENT/RESULT_DOCUMENT/";
            public const string AttachmentData = "ATTACHMENT/DATA/";
            public const string LetterFileName = "DOCUMENT.RTF";
            public const string ImageFileName = "IMAGE";
        }

        private static class ZedmedMeasurementType
        {
            public const string Height = "HEIGHT";
            public const string Weight = "WEIGHT";
            public const string HeadCircumference = "CIRCUM";
            public const string Waist = "WAIST";
            public const string BodyMassIndex = "BMI";
            public const string HeartRate = "RESP";
            public const string BloodPressureStandUp = "SYSSTAND";
            public const string BloodPressureStandDown = "DIASSTAND";
            public const string BloodPressureSitUp = "SYSTOLIC";
            public const string BloodPressureSitDown = "DIASTOLIC";
            public const string BloodPressureLieUp = "SYSLIE";
            public const string BloodPressureLieDown = "DIASLIE";
        }

        private static class ZedmedControlProperty
        {
            public const string Caption = "Caption";
            public const string ProgressNote = "ProgressNote";
            public const string TabOrder = "TabOrder";
            public const string Items = "Items";
        }

        private enum ZedmedControlType
        {
            Edit = 2,
            CheckBox = 3,
            RadioGroup = 4,
            Combobox = 5,
            DateTimePicker = 6,
            Memo = 7,
        }

        private static readonly Dictionary<ZedmedControlType, MigrationChecklistControlType> ControlTypeZedmedToSiberia = new Dictionary<ZedmedControlType, MigrationChecklistControlType>
        {
            { ZedmedControlType.Edit, MigrationChecklistControlType.TextBox },
            { ZedmedControlType.CheckBox, MigrationChecklistControlType.CheckBoxList },
            { ZedmedControlType.Memo, MigrationChecklistControlType.TextArea },
            { ZedmedControlType.RadioGroup, MigrationChecklistControlType.RadioButtonList },
            { ZedmedControlType.DateTimePicker, MigrationChecklistControlType.TextBox },
            { ZedmedControlType.Combobox, MigrationChecklistControlType.DropDownList },
        };

        private static readonly Dictionary<ZedmedControlType, Func<IGrouping<int, ZedmedTemplateControl>, MigrationDictionaryItem[]>> TemplateControlToValuesFunctions = new Dictionary<ZedmedControlType, Func<IGrouping<int, ZedmedTemplateControl>, MigrationDictionaryItem[]>>
        {
            { ZedmedControlType.Edit, x => new MigrationDictionaryItem[] { } },
            { ZedmedControlType.DateTimePicker, x => new MigrationDictionaryItem[] { } },
            { ZedmedControlType.Memo, x => new MigrationDictionaryItem[] { } },
            { ZedmedControlType.CheckBox, x => x.GetValueByProperty(ZedmedControlProperty.Caption).ToMigrationDictionayItems() },
            { ZedmedControlType.RadioGroup, x => x.GetValueByProperty(ZedmedControlProperty.Items).ToMigrationDictionayItems() },
            { ZedmedControlType.Combobox, x => x.GetValueByProperty(ZedmedControlProperty.Items).ToMigrationDictionayItems() },
        };

        private static readonly Dictionary<string, string> MaritalStatusZedmedToSiberia = new Dictionary<string, string>
        {
            { "S", "Never married" },
            { "M", "Married" },
            { "F", "De facto" },
            { "D", "Divorced" },
            { "W", "Widowed" },
            { "X", "Separated" },
        };

        private static readonly Dictionary<string, string> SmokingStatusZedmedToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "0", null },
            { "1", "Never smoker" },
            { "2", "Former smoker" },
            { "3", "Current every day smoker" },
        };

        private static readonly Dictionary<string, string> AccountTypeZedmedToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "0", "Private" },
            { "1", "Health Fund" },
            { "2", "DVA" },
            { "3", "DVA" },
        };

        internal static MigrationUser ToMigrationUser(this ZedmedUser zedmedUser)
        {
            return new MigrationUser
            {
                ExternalId = zedmedUser.Id,
                Salutation = zedmedUser.Title,
                FirstName = zedmedUser.GivenName,
                Surname = zedmedUser.FamilyName,
                IsReceptionist = false,
            };
        }

        internal static MigrationContact ToMigrationContact(this ZedmedContact zedmedContact)
        {
            return new MigrationContact
            {
                ExternalId = zedmedContact.Id,
                AddressLine1 = zedmedContact.WorkAddressLine1,
                AddressLine2 = zedmedContact.WorkAddressLine2,
                Email = zedmedContact.Email,
                Fax = zedmedContact.WorkFaxNumber,
                FirstName = zedmedContact.GivenName,
                HomePhone = zedmedContact.HomePhoneNumber,
                LastName = zedmedContact.FamilyName,
                MobilePhone = zedmedContact.MobilePhoneNumber,
                ProviderNo = zedmedContact.ProviderNumber,
                Salutation = zedmedContact.Title,
                Suburb = zedmedContact.WorkSuburbTown,
                PostCode = zedmedContact.PostCode,
                WorkPhone = zedmedContact.WorkPhoneNumber,
            };
        }

        internal static MigrationPatient ToMigrationPatient(this ZedmedPatient zedmedPatient, Dictionary<string, ZedmedAccountHolder> accountHolders, Dictionary<int, ZedmedPatientProblem[]> patientProblems)
        {
            var backgroundInfo = patientProblems
                .GetValueOrNull(zedmedPatient.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        x.OnsetDate.AsInvariantString().WithFieldName("Date"),
                        x.ProblemText.WithFieldName("Note"),
                    },
                    ",",
                    Environment.NewLine);

            var result = new MigrationPatient
            {
                ExternalId = zedmedPatient.Id.ToString(),
                AccountHolderExternalId = zedmedPatient.AccounPayerId,
                AccountType = zedmedPatient.PensionStatus.Translate(AccountTypeZedmedToSiberia),
                FirstName = zedmedPatient.GivenName,
                LastName = zedmedPatient.FamilyName,
                MiddleName = zedmedPatient.PopularName,
                EmergencyPersonName = zedmedPatient.EmergencyContactName,
                EmergencyPersonPhone = GetActivePhone(zedmedPatient.MobilePhone, zedmedPatient.HomePhone, zedmedPatient.WorkPhone),
                Gender = zedmedPatient.Gender.EqualsIgnoreCase("M"),
                DateOfBirth = zedmedPatient.DateOfBirth,
                AddressLine1 = zedmedPatient.HomeAddressLine1,
                AddressLine2 = zedmedPatient.HomeAddressLine2,
                Suburb = zedmedPatient.HomeSuburbTown,
                PostCode = zedmedPatient.HomePostCode,
                HomePhone = zedmedPatient.HomePhone,
                WorkPhone = zedmedPatient.WorkPhone,
                MobilePhone = zedmedPatient.MobilePhone,
                Email = zedmedPatient.EmailAddress,
                SmokingNotes = zedmedPatient.SmokingDetails,
                SmokingStatus = zedmedPatient.SmokingStatus.Translate(SmokingStatusZedmedToSiberia),
                DrinkingNotes = zedmedPatient.AlcoholDetails,
                DoNotSendSms = zedmedPatient.AllowSms.EqualsIgnoreCase("N"),
                Occupation = zedmedPatient.Occupation,
                NextOfKinName = zedmedPatient.NokName,
                NextOfKinContactPhone = GetActivePhone(zedmedPatient.NokMobilePhone, zedmedPatient.NokHomePhone, zedmedPatient.NokWorkPhone),
                Alert = zedmedPatient.Alerts,
                MaritalStatus = zedmedPatient.MaritalStatus.Translate(MaritalStatusZedmedToSiberia),
                DvaNumber = zedmedPatient.VeteranAffairsNumber,
                HccNumber = zedmedPatient.HealthCareCard,
                HccExpiry = zedmedPatient.HealthCareCardExpiryDate.AsInvariantString(),
                PreviousIssues = string.Join(Environment.NewLine, zedmedPatient.FamilyHistory, zedmedPatient.SocialHistory, zedmedPatient.PatientNotes),
                BackgroundInfo = backgroundInfo,
                UsualProvider = zedmedPatient.UsualClinic,
                UsualGpExternalId = zedmedPatient.UsualDoctor,
            };

            var accountHolder = accountHolders.GetValueOrNull(zedmedPatient.AccounPayerId);
            if (accountHolder != null)
            {
                result.GuardianName = accountHolder.GivenName;
                result.GuardianSurname = accountHolder.FamilyName;
                result.GuardianHomePhone = accountHolder.HomePhone;
                result.GuardianState = accountHolder.StatusCode;
            }

            if (!string.IsNullOrWhiteSpace(zedmedPatient.MedicareNumber))
            {
                string mediacare = zedmedPatient.MedicareNumber;
                int refNum;
                if (int.TryParse(mediacare.Last().ToString(), out refNum))
                {
                    result.MedicareExpiryDate = zedmedPatient.MedicareNumberExpiryDate;
                    result.MedicareRefNum = refNum;
                    result.MedicareNum = mediacare.Substring(0, mediacare.Length - 1);
                }
            }

            return result;
        }

        internal static MigrationAccountHolder ToMigrationAccountHolder(this ZedmedAccountHolder zedmedAccountHolder, Dictionary<string, int[]> zedmedPatients)
        {
            return new MigrationAccountHolder
            {
                ExternalId = zedmedAccountHolder.Id,
                AddressLine1 = zedmedAccountHolder.HomeAddressLine1,
                AddressLine2 = zedmedAccountHolder.HomeAddressLine2,
                DateOfBirth = zedmedAccountHolder.DateOfBirth,
                FirstName = zedmedAccountHolder.GivenName,
                HomePhone = zedmedAccountHolder.HomePhone,
                MedicareExpiryDate = zedmedAccountHolder.MedicareNumberExpiry,
                MedicareNum = zedmedAccountHolder.MedicareNumber,
                PostCode = zedmedAccountHolder.HomePostCode,
                State = zedmedAccountHolder.StatusCode,
                Suburb = zedmedAccountHolder.HomeSuburbTown,
                LastName = zedmedAccountHolder.FamilyName,
                Salutation = zedmedAccountHolder.Title,
                Individual = true,
                PatientExternalIds = zedmedPatients.GetValueOrDefault(zedmedAccountHolder.Id, new int[] { }).Select(x => x.ToString()).ToArray(),
            };
        }

        internal static MigrationCompany ToMigrationCompany(this ZedmedAccountPayer zedmedAccountPayer)
        {
            return new MigrationCompany
            {
                AddressLine1 = zedmedAccountPayer.AddressLine1,
                AddressLine2 = zedmedAccountPayer.AddressLine2,
                Name = zedmedAccountPayer.Name,
                ContactPersonName = zedmedAccountPayer.ContactName,
                Fax = zedmedAccountPayer.FaxNumber,
                ExternalId = zedmedAccountPayer.Id,
                Phone = zedmedAccountPayer.ContactPhone,
                Phone2 = zedmedAccountPayer.PhoneNumber,
                Suburb = zedmedAccountPayer.SuburbTown,
                PostCode = zedmedAccountPayer.PostCode,
            };
        }

        internal static MigrationReferral ToMigrationReferral(this ZedmedReferral zedmedReferral)
        {
            return new MigrationReferral
            {
                ExternalId = zedmedReferral.Id.ToString(),
                PatientExternalId = zedmedReferral.PatientId.ToString(),
                ContactExternalId = zedmedReferral.ReferralDoctortId.ToString(),
                ReferralDate = zedmedReferral.ReferralDate,
                IssueDate = zedmedReferral.LetterDate,
                Duration = zedmedReferral.ReferralPeriod,
            };
        }

        internal static MigrationAppointmentType ToMigrationAppointmentType(this ZedmedAppointmentType zedmedAppointmentType)
        {
            return new MigrationAppointmentType
            {
                ExternalId = zedmedAppointmentType.Id.ToString(),
                Name = zedmedAppointmentType.Description,
                Colour = ColorTranslator.ToHtml(FromBgrToRgb(zedmedAppointmentType.BackgroundColour)),
                Duration = (zedmedAppointmentType.Duration.TimeOfDay.Hours * 60) + zedmedAppointmentType.Duration.TimeOfDay.Minutes,
                IsDeactivated = !zedmedAppointmentType.IsActive.EqualsIgnoreCase("Y"),
            };
        }

        internal static MigrationAppointment ToMigrationAppointment(this ZedmedAppointment zedmedAppointment)
        {
            return new MigrationAppointment
            {
                ExternalId = zedmedAppointment.Id.ToString(),
                PatientExternalId = zedmedAppointment.PatientId.ToString(),
                UserExternalId = zedmedAppointment.DoctorCode,
                Description = zedmedAppointment.Notes,
                AppointmentTypeExternalId = zedmedAppointment.AppointmentTypeId.ToString(),
                CreationDate = zedmedAppointment.BookingTime ?? DateTime.Now,
                StartDateTime = zedmedAppointment.StartPoint ?? DateTime.Now,
                EndDateTime = zedmedAppointment.EndPoint ?? DateTime.Now,
                CancellationReasonId = zedmedAppointment.StatusCode.EqualsIgnoreCase("CA") ? AppointmentCancelledByPatient : null,
            };
        }

        internal static MigrationConsult ToMigrationConsult(this ZedmedEncounter zedmedEncounter, Dictionary<int, ZedmedEncounterNote[]> encounterNotes, Regex checklistHtmlTagRegex)
        {
            var findings = encounterNotes
                .GetValueOrNull(zedmedEncounter.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        zedmedEncounter.ConvertedData.EqualsIgnoreCase("Y")
                            ? x.SectionNotes
                            : checklistHtmlTagRegex.Replace(WebUtility.HtmlDecode(x.SectionNotes), string.Empty).RemoveHtmlTag()
                    },
                    ": ",
                    ", ");

            return new MigrationConsult
            {
                ExternalId = zedmedEncounter.Id.ToString(),
                PatientExternalId = zedmedEncounter.PatientId.ToString(),
                UserExternalId = zedmedEncounter.DoctorCode,
                StartConsult = zedmedEncounter.StartDateTime ?? DateTime.MaxValue,
                Findings = findings,
            };
        }

        internal static MigrationAllergy ToMigrationAllergy(this IGrouping<int, string> zedmedAllergies)
        {
            return new MigrationAllergy
            {
                Allergies = zedmedAllergies.ToArray(),
                PatientExternalId = zedmedAllergies.Key.ToString(),
            };
        }

        internal static MigrationMeasurement ToMigrationMeasurement(this IGrouping<string, ZedmedMeasurement> zedmedMeasurements)
        {
            var bloodPressureStandUp = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BloodPressureStandUp);
            var bloodPressureStandDown = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BloodPressureStandDown);
            var bloodPressureSitUp = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BloodPressureSitUp);
            var bloodPressureSitDown = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BloodPressureSitDown);
            var bloodPressureLieUp = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BloodPressureLieUp);
            var bloodPressureLieDown = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BloodPressureLieDown);

            return new MigrationMeasurement
            {
                ExternalId = zedmedMeasurements.Key,
                CreatedDate = zedmedMeasurements.Select(x => x.MeasureTime).FirstOrDefault(),
                PatientExternalId = zedmedMeasurements.Select(x => x.PatientId).FirstOrDefault().ToString(),
                Height = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.Height).GetNullableFloat(),
                Weight = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.Weight).GetNullableFloat(),
                HeadCircumference = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.HeadCircumference).GetNullableFloat(),
                Waist = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.Waist).GetNullableFloat(),
                BodyMassIndex = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.BodyMassIndex).GetNullableFloat(),
                HeartRate = zedmedMeasurements.GetValueByType(ZedmedMeasurementType.HeartRate).GetNullableInt(),
                BloodPressureUp = bloodPressureSitUp.GetNullableInt() ?? bloodPressureStandUp.GetNullableInt() ?? bloodPressureLieUp.GetNullableInt(),
                BloodPressureDown = bloodPressureSitDown.GetNullableInt() ?? bloodPressureStandDown.GetNullableInt() ?? bloodPressureLieDown.GetNullableInt(),
            };
        }

        internal static MigrationTask ToMigrationTask(this ZedmedTask zedmedTask, Dictionary<string, string> users)
        {
            return new MigrationTask
            {
                ExternalId = zedmedTask.Id.ToString(),
                CreatedDate = zedmedTask.EnteredDateTime ?? new DateTime(1900, 1, 1),
                Subject = zedmedTask.Comments,
                DueDate = zedmedTask.DueDate ?? zedmedTask.LastPerformedDateTime ?? DateTime.MaxValue,
                FromUserExternalId = users.ContainsKey(zedmedTask.UserName) ? users[zedmedTask.UserName] : null,
                PatientExternalId = zedmedTask.PatientId,
                ToUserExternalId = users.ContainsKey(zedmedTask.EnteredBy) ? users[zedmedTask.EnteredBy] : null,
            };
        }

        internal static MigrationVaccination ToMigrationVaccination(this ZedmedVaccination zedmedVaccination)
        {
            return new MigrationVaccination
            {
                ACIRCode = zedmedVaccination.ACIRCode,
                Dose = zedmedVaccination.Seq,
                Name = zedmedVaccination.ImmDesc.NullIfEmpty() ?? zedmedVaccination.ManualImm,
                PatientExternalId = zedmedVaccination.PatientId.ToString(),
                VaccinationDate = zedmedVaccination.ImmDateTime,
            };
        }

        internal static MigrationDocument ToMigrationDocument(this ZedmedAttachment zedmedAttachment)
        {
            return new MigrationDocument
            {
                ExternalId = zedmedAttachment.Id.ToString(),
                Description = zedmedAttachment.Description,
                ImageDate = zedmedAttachment.SavedDateTime,
                PatientExternalId = zedmedAttachment.PatientId.ToString(),
                FileName = zedmedAttachment.FileName,
                Md5 = ZedmedDocumentDirectory.AttachmentData,
            };
        }

        internal static MigrationDocument ToMigrationDocument(this ZedmedImageDocument zedmedImageDocument)
        {
            return new MigrationDocument
            {
                ExternalId = zedmedImageDocument.Id.ToString(),
                Description = zedmedImageDocument.ImageDesc,
                ImageDate = zedmedImageDocument.SavedDateTime,
                PatientExternalId = zedmedImageDocument.PatientId.ToString(),
                FileName = Path.ChangeExtension(ZedmedDocumentDirectory.ImageFileName, zedmedImageDocument.FileExtension),
                Md5 = ZedmedDocumentDirectory.AttachmentData,
            };
        }

        internal static MigrationChecklistTemplate ToMigrationChecklistTemplate(this ZedmedChecklistTemplate checklistTemplate, Dictionary<int, ZedmedTemplateControl[]> templateControls)
        {
            var items = templateControls.GetValueOrDefault(checklistTemplate.Id, new ZedmedTemplateControl[] { })
                .GroupBy(x => x.ControlId)
                .Select(x => x.ToMigrationChecklistTemplateItem());

            return new MigrationChecklistTemplate
            {
                ExternalId = checklistTemplate.Id.ToString(),
                Name = checklistTemplate.Name,
                Items = items.ToArray(),
            };
        }

        internal static MigrationChecklist ToMigrationChecklist(this ZedmedChecklist zedmedChecklist, Dictionary<int, ZedmedTemplateControl[]> templateControls)
        {
            var checklistControls = templateControls.GetValueOrDefault(zedmedChecklist.TemplateId, new ZedmedTemplateControl[] { })
                .GroupBy(x => x.ControlId)
                .ToArray();

            var fieldNameWithValue = zedmedChecklist.ToFieldNameWithValue(checklistControls);

            var items = checklistControls
                .Select(x => x.ToMigrationChecklistItem(fieldNameWithValue));

            return new MigrationChecklist
            {
                ExternalId = zedmedChecklist.Id,
                Name = zedmedChecklist.Name,
                CreatedDate = zedmedChecklist.DateCreated ?? DateTime.Today,
                PatientExternalId = zedmedChecklist.PatientId.ToString(),
                DoctorExternalId = zedmedChecklist.DoctorId,
                Items = items.ToArray(),
            };
        }

        internal static MigrationLetter ToMigrationLetter(this ZedmedLetter zedmedLetter)
        {
            return new MigrationLetter
            {
                ExternalId = zedmedLetter.Id,
                AuthorExternalId = zedmedLetter.FromDoctorCode,
                ContactExternalId = zedmedLetter.PrimaryRecipient.ToString(),
                Date = zedmedLetter.LetterDate ?? zedmedLetter.SavedDateTime ?? DateTime.MaxValue,
                PatientExternalId = zedmedLetter.PatientId.ToString(),
                Text = zedmedLetter.DocumentId.ToString(),
                Status = MigrationLetterStatus.Sent,
            };
        }

        internal static MigrationInterestedParty ToMigrationInterestedParty(this ZedmedInterestedParty zedmedInterestedParty)
        {
            return new MigrationInterestedParty
            {
                ExternalId = zedmedInterestedParty.Id,
                ContactExternalId = zedmedInterestedParty.AddressBookId.ToString(),
                PatientExternalId = zedmedInterestedParty.PatientId.ToString(),
            };
        }

        internal static MigrationRecall ToMigrationRecall(this ZedmedRecall zedmedRecall)
        {
            var zedmedUsualPeriod = zedmedRecall.UsualPeriod ?? string.Empty;
            int recurrenceInterval;
            if (int.TryParse(zedmedUsualPeriod.Substring(0, zedmedUsualPeriod.Length - 1), out recurrenceInterval))
            {
                switch (zedmedUsualPeriod.Last())
                {
                    case 'Y':
                        recurrenceInterval *= 12;
                        break;

                    case 'W':
                        recurrenceInterval /= 4;
                        break;

                    case 'M':
                        break;

                    default:
                        break;
                }
            }

            return new MigrationRecall
            {
                ExternalId = zedmedRecall.Id,
                PatientExternalId = zedmedRecall.PatientId,
                Reason = zedmedRecall.RecallTypeDescription,
                IsCompleted = zedmedRecall.OnGoing.EqualsIgnoreCase("Y"),
                DueDate = zedmedRecall.CreationDate,
                PatientPhone = GetActivePhone(zedmedRecall.MobilePhone, zedmedRecall.HomePhone, zedmedRecall.WorkPhone),
                NextCallDate = zedmedRecall.AttendanceDate,
                DoctorFullName = ToFullName(zedmedRecall.Title, zedmedRecall.GivenName, zedmedRecall.FamilyName),
                DateEntered = zedmedRecall.CreationDate,
                RecurrenceInterval = recurrenceInterval,
            };
        }

        private static string ToFullName(string salutation, string firstName, string lastName)
        {
            var names = new[] { salutation, firstName, lastName };

            return string.Join(" ", names.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        internal static MigrationScript ToMigrationScript(this ZedmedScript zedmedScript, Dictionary<string, ZedmedDrug> drugs)
        {
            var drug = drugs.ContainsKey(zedmedScript.DrugId)
                ? ToMigrationDrug(drugs[zedmedScript.DrugId])
                : null;

            return new MigrationScript
            {
                ExternalId = zedmedScript.Id,
                PatientExternalId = zedmedScript.PatientId.ToString(),
                ExternalDrugId = zedmedScript.DrugId,
                MigrationDrug = drug,
                AuthorityNumber = zedmedScript.AuthorityNumber,
                Comments = zedmedScript.Notes,
                Created = zedmedScript.DateFirstPrescribed.AsInvariantString(),
                Dose = zedmedScript.DosageFullText,
                OtherMedicalDescription = zedmedScript.ScriptDesc,
                RepeatDays = zedmedScript.Repeats,
                ExternalUserId = zedmedScript.DoctorCode,
            };
        }

        internal static MigrationDrug ToMigrationDrug(ZedmedDrug zedmedDrug)
        {
            return new MigrationDrug
            {
                ExternalId = zedmedDrug.Id,
                Composition = zedmedDrug.Quantity,
                Name = zedmedDrug.ShortDesc,
                MimsProdCode = zedmedDrug.ProdCode,
                MimsFormCode = zedmedDrug.FormCode,
                MimsPackCode = zedmedDrug.PackCode,
            };
        }

        internal static MigrationLaboratoryResult ToMigrationLaboratoryResult(this ZedmedLaboratoryResult zedmedLaboratoryResult, Dictionary<int, ZedmedPatient> zedmedPatients)
        {
            string firstName = string.Empty;
            string surname = string.Empty;
            DateTime? dateOfbirth = null;
            var zedmedPatient = zedmedPatients.GetValueOrNull(zedmedLaboratoryResult.PatientId);
            if (zedmedPatient != null)
            {
                firstName = zedmedPatient.GivenName;
                surname = zedmedPatient.FamilyName;
                dateOfbirth = zedmedPatient.DateOfBirth;
            }

            return new MigrationLaboratoryResult
            {
                ExternalId = zedmedLaboratoryResult.Id,
                PatientExternalId = zedmedLaboratoryResult.PatientId.ToString(),
                DoctorExternalId = zedmedLaboratoryResult.DoctorCode,
                LaboratoryResultType = "Pathology",
                PatientName = firstName,
                PatientLastName = surname,
                PatientDateOfBirth = dateOfbirth ?? new DateTime(1900, 1, 1),
                ResultName = zedmedLaboratoryResult.Description,
                ImportDate = zedmedLaboratoryResult.SavedDateTime ?? zedmedLaboratoryResult.ReportedDate ?? zedmedLaboratoryResult.ReceivedDate ?? zedmedLaboratoryResult.CollectedDate,
                FormattedResult = zedmedLaboratoryResult.EdocId,
            };
        }

        internal static MigrationWorkCoverClaim ToMigrationWorkCoverClaim(this ZedmedWorkCoverClaim zedmedWorkCoverClaim)
        {
            return new MigrationWorkCoverClaim
            {
                ExternalId = zedmedWorkCoverClaim.Id,
                PatientExternalId = zedmedWorkCoverClaim.PatientId,
                EmployerCompanyExternalId = zedmedWorkCoverClaim.AccountPayerId,
                EmployerRepresentativeName = zedmedWorkCoverClaim.Employer,
                InsuranceCompanyExternalId = zedmedWorkCoverClaim.AccountPayerId,
                ClaimManagerName = zedmedWorkCoverClaim.Name,
                ClaimManagerPhone = zedmedWorkCoverClaim.ContactPhone,
                ClaimId = zedmedWorkCoverClaim.InsuranceClaimNumber,
                DateOfInjury = zedmedWorkCoverClaim.EntryDate.AsInvariantString(),
                TimeOfInjury = (zedmedWorkCoverClaim.EntryDate.HasValue ? zedmedWorkCoverClaim.EntryDate.Value.TimeOfDay : (TimeSpan?)null).AsDisplayTimeString(),
                LocationOnBody = string.Empty,
                LocationOfInjury = string.Empty,
            };
        }

        public static string GetPatientDocumentZipFilePath(this string zedmedPatientId, string zedmedDocumentsPath)
        {
            int patientId = zedmedPatientId.GetNullableInt() ?? 0;

            int patientIdStart = patientId - (patientId % 1000);
            int patientIdEnd = patientIdStart + 999;

            var parentDirectory = PadLeftPatientIdByZero(patientIdStart) + "-" + PadLeftPatientIdByZero(patientIdEnd);
            var zipFileName = PadLeftPatientIdByZero(patientId);

            try
            {
                string zipFilePath = Path.Combine(zedmedDocumentsPath, parentDirectory, zipFileName);
                return File.Exists(zipFilePath) ? zipFilePath : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string PadLeftPatientIdByZero(int patientId)
        {
            return patientId.ToString().PadLeft(ZedmedPatientIdMaxDigit, '0');
        }

        private static string GetValueByType(this IGrouping<string, ZedmedMeasurement> zedmedMeasurements, string measurementType)
        {
            var measureValue = zedmedMeasurements.FirstOrDefault(x => x.MeasureDesc.EqualsIgnoreCase(measurementType)) ?? new ZedmedMeasurement();

            return measureValue.MeasureValue;
        }

        private static MigrationChecklistTemplateItem ToMigrationChecklistTemplateItem(this IGrouping<int, ZedmedTemplateControl> controls)
        {
            var controlType = (ZedmedControlType)controls.Select(o => o.ControlTypeId).FirstOrDefault();

            return new MigrationChecklistTemplateItem
            {
                ControlName = controls.GetValueByProperty(ZedmedControlProperty.ProgressNote).NullIfEmpty() ?? controls.GetValueByProperty(ZedmedControlProperty.Caption),
                ControlType = ControlTypeZedmedToSiberia.ContainsKey(controlType) ? ControlTypeZedmedToSiberia[controlType] : 0,
                Values = TemplateControlToValuesFunctions.GetValueOrNull(controlType)(controls) ?? new MigrationDictionaryItem[] { },
            };
        }

        private static MigrationChecklistItem ToMigrationChecklistItem(this IGrouping<int, ZedmedTemplateControl> controls, Dictionary<string, string> fieldNameWithValues)
        {
            var controlType = (ZedmedControlType)controls.Select(o => o.ControlTypeId).FirstOrDefault();
            var controlValues = TemplateControlToValuesFunctions.GetValueOrNull(controlType)(controls);
            var fieldName = controls.GetValueByProperty(ZedmedControlProperty.ProgressNote).NullIfEmpty() ?? controls.GetValueByProperty(ZedmedControlProperty.Caption);
            var fieldValue = fieldNameWithValues.GetValueOrNull(fieldName);

            int sortOrder;
            if (!int.TryParse(controls.GetValueByProperty(ZedmedControlProperty.TabOrder), out sortOrder))
            {
                sortOrder = 0;
            }

            return new MigrationChecklistItem
            {
                SortOrder = sortOrder,
                ControlName = fieldName,
                ControlType = ControlTypeZedmedToSiberia.ContainsKey(controlType) ? ControlTypeZedmedToSiberia[controlType] : 0,
                Values = controlValues.Any() ? controlValues : fieldValue.ToMigrationDictionayItems(),
                SelectedValueIds = controlType == ZedmedControlType.CheckBox
                    ? fieldValue.EqualsIgnoreCase("Yes")
                        ? new[] { 0 }
                        : new int[] { }
                    : controlValues
                        .Where(x => fieldValue.EqualsIgnoreCase(x.Text))
                        .Select(x => x.Id)
                        .ToArray(),
            };
        }

        private static MigrationDictionaryItem[] ToMigrationDictionayItems(this string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                ? value
                    .Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select((x, index) => new MigrationDictionaryItem
                    {
                        Id = index,
                        Text = x,
                    })
                    .ToArray()
                : new MigrationDictionaryItem[] { };
        }

        private static string GetValueByProperty(this IGrouping<int, ZedmedTemplateControl> controls, string property)
        {
            var controlProperty = controls.FirstOrDefault(x => x.Property.EqualsIgnoreCase(property)) ?? new ZedmedTemplateControl();

            return controlProperty.Value;
        }

        private static Dictionary<string, string> ToFieldNameWithValue(this ZedmedChecklist zedmedChecklist, IGrouping<int, ZedmedTemplateControl>[] checklistControls)
        {
            var checklistFieldNameWithValueRegex = new Regex(string.Format(
                ChecklistFieldNameWithValuePattern,
                checklistControls.Select(x => Regex.Escape(x.GetValueByProperty(ZedmedControlProperty.ProgressNote))).AsStringAlternation()));

            return checklistFieldNameWithValueRegex
                .Matches(zedmedChecklist.SectionNotes)
                .Cast<Match>()
                .Select(x => new
                {
                    Name = x.Groups[1].Value,
                    Value = x.Groups[3].Value,
                })
                .GroupBy(x => x.Name)
                .Select(x => x.FirstOrDefault())
                .ToDictionary(x => x.Name, x => x.Value);
        }

        private static Color FromBgrToRgb(int colorBgrDecimal)
        {
            var bytes = BitConverter.GetBytes(colorBgrDecimal);
            return Color.FromArgb(bytes[0], bytes[1], bytes[2]);
        }

        public static string AsStringAlternation<T>(this IEnumerable<T> source)
        {
            return source == null
                ? null
                : string.Join(
                    "|",
                    source
                        .Select(x => x.ToString())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                    );
        }

        private static string GetActivePhone(string mobilePhone, string homePhone, string workPhone)
        {
            return new[] { mobilePhone, homePhone, workPhone }.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }
    }
}