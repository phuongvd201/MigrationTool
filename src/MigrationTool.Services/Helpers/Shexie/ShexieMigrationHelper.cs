using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using MigrationTool.Services.Entities.Shexie;
using MigrationTool.Services.Helpers.Text;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Helpers.Shexie
{
    internal static class ShexieMigrationHelper
    {
        private const string AppointmentCancelledByPatient = "1";

        public enum ShexieLabResultCompressed
        {
            RtfText = 0,
            HtmlFilePath = 9,
        }

        public enum ShexieHistoryNoteFormatted
        {
            PlainTextForPatientNote = 0,
            RtfTextForPatientNote = 1,
            RtfTextForConsultHistory = 2,
        }

        private static readonly Dictionary<string, string> SalutationShexieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
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
        };

        internal static MigrationUser ToMigrationUser(this ShexieUser shexieUser)
        {
            return new MigrationUser
            {
                ExternalId = shexieUser.Id,
                FirstName = shexieUser.FirstName,
                Surname = shexieUser.Surname,
                IsReceptionist = true,
            };
        }

        internal static MigrationUser ToMigrationUser(this ShexieProvider shexieProvider)
        {
            var parsedName = shexieProvider.ProviderName.ParseName(SalutationShexieToSiberia.Keys.ToArray());

            return new MigrationUser
            {
                ExternalId = shexieProvider.Provider,
                Salutation = (shexieProvider.Salutation.NullIfEmpty() ?? parsedName.Salutation.NullIfEmpty() ?? "Mr").Translate(SalutationShexieToSiberia),
                FirstName = shexieProvider.FirstName.NullIfEmpty() ?? parsedName.FirstName,
                Surname = shexieProvider.Surname.NullIfEmpty() ?? parsedName.LastName,
                IsReceptionist = false,
            };
        }

        internal static MigrationContact ToMigrationContact(this ShexieContact shexieContact)
        {
            return new MigrationContact
            {
                ExternalId = shexieContact.Id.ToString(),
                AddressLine1 = shexieContact.AddressLine1,
                AddressLine2 = shexieContact.AddressLine2,
                LastName = shexieContact.LastName,
                HomePhone = shexieContact.HomePhone,
                MobilePhone = shexieContact.MobilePhone,
                Fax = shexieContact.Fax,
                Salutation = shexieContact.Salutation,
                FirstName = shexieContact.FirstName,
                ProviderNo = shexieContact.ProviderNo,
                Email = shexieContact.Email,
                Suburb = shexieContact.Suburb,
                PostCode = shexieContact.PostCode,
                State = shexieContact.State,
                Country = shexieContact.Country,
            };
        }

        internal static MigrationCompany ToMigrationCompany(this ShexieCompany shexieCompany)
        {
            return new MigrationCompany
            {
                ExternalId = shexieCompany.Id.ToString(),
                AddressLine1 = shexieCompany.AddressLine1,
                AddressLine2 = shexieCompany.AddressLine2,
                Fax = shexieCompany.Fax,
                Name = shexieCompany.Name,
                Phone = shexieCompany.Phone,
                Suburb = shexieCompany.Suburb,
                PostCode = shexieCompany.PostCode,
                State = shexieCompany.State,
                Country = shexieCompany.Country,
            };
        }

        internal static MigrationOpReport ToMigrationOpReport(this ShexieOpReport shexieOpReport)
        {
            return new MigrationOpReport
            {
                ExternalId = shexieOpReport.Id.ToString(),
                PatientExternalId = shexieOpReport.PatientId.ToString(),
                AnaesthetistExternalId = shexieOpReport.AnaesthetistId.ToString(),
                AssistantExternalId = shexieOpReport.AssistantId.ToString(),
                Indication = shexieOpReport.Details,
                DoctorExternalId = shexieOpReport.Provider,
                ProcedureDate = shexieOpReport.ShexieHospitalList.SurgeryDate.AsInvariantString() ?? DateTime.Now.Date.AsInvariantString(),
                ProcedureTimeFrom = (shexieOpReport.ShexieHospitalList.SurgeryTime ?? DateTime.Now.Date).TimeOfDay.AsInvariantString(),
                ProcedureTimeTo = (shexieOpReport.ShexieHospitalList.SurgeryEndTime ?? DateTime.Now.Date).TimeOfDay.AsInvariantString(),
                Admission = shexieOpReport.ShexieHospitalList.Admission.AsInvariantString(),
                DischargeDate = shexieOpReport.ShexieHospitalList.DischargeDate.AsInvariantString(),
                ProcedureName = string.Join(" ", shexieOpReport.FeeEstimateItems.Select(p => p.Description).ToArray()),
                Finding = shexieOpReport.Finding,
                PostOp = shexieOpReport.Details,
                QuoteItems = shexieOpReport.FeeEstimateItems.Select(x => new MigrationQuoteItem
                {
                    ExternalId = x.Id.ToString(),
                    OpReportExternalId = shexieOpReport.Id,
                    ItemNumber = x.ItemNumber,
                    Icd10Code = x.Icd10Code,
                    Description = x.Description,
                }).ToArray(),
            };
        }

        internal static MigrationLaboratoryResult ToMigrationLaboratoryResult(this ShexieLaboratoryResult shexieLabResult, string documentsPath)
        {
            var formattedResult = shexieLabResult.FormatLabResultData(documentsPath);

            return new MigrationLaboratoryResult
            {
                ExternalId = shexieLabResult.Id + "_" + shexieLabResult.RunId,
                PatientExternalId = shexieLabResult.PatientId.ToString(),
                DoctorExternalId = shexieLabResult.Provider,
                LaboratoryResultType = shexieLabResult.ResultType.Description,
                PatientName = shexieLabResult.Patient.FirstName,
                PatientLastName = shexieLabResult.Patient.LastName,
                PatientDateOfBirth = shexieLabResult.Patient.DateOfBirth ?? DateTime.MaxValue,
                ResultName = shexieLabResult.ResultName,
                FormattedResult = formattedResult,
                ImportDate = shexieLabResult.DateCollected,
                AbnormalStatus = shexieLabResult.Normal,
            };
        }

        internal static MigrationTask ToMigrationTask(this ShexieAlarm shexieAlarm)
        {
            return new MigrationTask
            {
                ExternalId = shexieAlarm.Id.ToString(),
                FromUserExternalId = shexieAlarm.FromUserId,
                ToUserExternalId = shexieAlarm.ToUserId,
                PatientExternalId = shexieAlarm.PatientId.ToString(),
                DueDate = shexieAlarm.IsFinished == 1
                    ? shexieAlarm.DueDate.HasValue
                        ? shexieAlarm.DueDate.Value.Date + (shexieAlarm.DueTime.HasValue ? shexieAlarm.DueTime.Value.TimeOfDay : DateTime.MinValue.TimeOfDay)
                        : DateTime.MaxValue
                    : DateTime.Today,
                CreatedDate = DateTime.Today,
                IsFinished = shexieAlarm.IsFinished == 1,
                Subject = shexieAlarm.Description,
            };
        }

        internal static MigrationPatient ToMigrationPatient(
            this ShexiePatient shexiePatient,
            Dictionary<int, ShexiePatientHistory[]> notes,
            Dictionary<int, ShexieStatistic[]> statistics,
            Dictionary<int, ShexieAnalysis> healthConditionAnalyses)
        {
            var combinedNotes = notes
                .GetValueOrNull(shexiePatient.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        x.Date.AsDisplayDateString().WithFieldName("Date"),
                        FormatNoteData(x).WithFieldName("Note"),
                    },
                    "," + Environment.NewLine,
                    ";" + Environment.NewLine);

            var previousIssues = statistics
                .GetValueOrNull(shexiePatient.Id)
                .EmptyIfNull()
                .Where(x => healthConditionAnalyses.ContainsKey(x.Type))
                .AsSeparatedString(
                    x => new[]
                    {
                        x.Analysis.Desc
                    },
                    ",",
                    Environment.NewLine);

            var migrationPatient =
                new MigrationPatient
                {
                    ExternalId = shexiePatient.Id.ToString(),
                    LastName = shexiePatient.LastName,
                    MiddleName = shexiePatient.MiddleName,
                    FirstName = shexiePatient.FirstName,
                    AddressLine1 = shexiePatient.AddressLine1,
                    AddressLine2 = shexiePatient.AddressLine2,
                    HomePhone = shexiePatient.HomePhone,
                    WorkPhone = shexiePatient.WorkPhone,
                    MobilePhone = shexiePatient.MobilePhone,
                    DateOfBirth = shexiePatient.DateOfBirth,
                    MedicareNum = shexiePatient.MedicareNum,
                    MedicareRefNum = int.Parse(shexiePatient.CardNo.ToString()),
                    MedicareExpiryDate = shexiePatient.MedicareExpiryDate,
                    Gender = shexiePatient.Gender == "M",
                    DoNotSendSms = shexiePatient.NoCorrespondence != 0,
                    Suburb = shexiePatient.Suburb,
                    PostCode = shexiePatient.PostCode,
                    State = shexiePatient.State,
                    Country = shexiePatient.Country,
                    CountryOfBirth = shexiePatient.PatientSecondary.Country.CountryName,
                    Language = string.Empty,
                    MaritalStatus = string.Empty,
                    Email = shexiePatient.PatientSecondary.Email,
                    AccountType = string.Empty,
                    GuardianName = shexiePatient.Guardian.FirstName,
                    GuardianSurname = shexiePatient.Guardian.LastName,
                    GuardianHomePhone = shexiePatient.Guardian.HomePhone,
                    GuardianState = shexiePatient.Guardian.State,
                    EmergencyPersonName = ToFullName(string.Empty, shexiePatient.EmergencyPerson.FirstName, shexiePatient.EmergencyPerson.LastName),
                    EmergencyPersonPhone = GetActivePhone(shexiePatient.EmergencyPerson.MobilePhone, shexiePatient.EmergencyPerson.HomePhone),
                    HealthFundNumber = shexiePatient.PatientSecondary.HealthFundMemberNo,
                    HealthFundCode = shexiePatient.Company.HealthFundCode,
                    HealthFundName = shexiePatient.Company.HealthFundName,
                    NextOfKinAddress = shexiePatient.EmergencyPerson.Address,
                    NextOfKinContactPhone = GetActivePhone(shexiePatient.EmergencyPerson.MobilePhone, shexiePatient.EmergencyPerson.HomePhone),
                    NextOfKinName = ToFullName(shexiePatient.EmergencyPerson.Salutation, shexiePatient.EmergencyPerson.FirstName, shexiePatient.EmergencyPerson.LastName),
                    DvaNumber = shexiePatient.VetAffNo,
                    HccPensionNumber = shexiePatient.PensionNo,
                    HccNumber = shexiePatient.HCCNo,
                    HccExpiry = shexiePatient.HCCExpiry.AsInvariantString(),
                    Occupation = shexiePatient.Occupation,
                    PreviousIssues = previousIssues,
                    BackgroundInfo = shexiePatient.PatientSecondary.BackGroundInfo,
                    Notes = combinedNotes,
                };
            return migrationPatient;
        }

        internal static MigrationReferral ToMigrationReferral(this ShexieReferral shexieReferral)
        {
            return new MigrationReferral
            {
                ExternalId = shexieReferral.Id.ToString(),
                PatientExternalId = shexieReferral.PatientId.ToString(),
                ContactExternalId = shexieReferral.ContactId.ToString(),
                ReferralDate = shexieReferral.ReferralDate,
                IssueDate = shexieReferral.IssueDate,
                Duration = shexieReferral.Duration,
            };
        }

        internal static MigrationAppointmentType ToMigrationAppointmentType(this ShexieAppointmentType shexieAppointmentType)
        {
            return new MigrationAppointmentType
            {
                ExternalId = shexieAppointmentType.Id.ToString(),
                Name = shexieAppointmentType.Name,
                Colour = ColorTranslator.ToHtml(Color.FromArgb(shexieAppointmentType.Colour)),
                Duration = shexieAppointmentType.Duration,
            };
        }

        internal static MigrationAppointment ToMigrationAppointment(this ShexieAppointment shexieAppointment)
        {
            var startDateTime = (shexieAppointment.StartDate ?? DateTime.MinValue).Date + (shexieAppointment.StartTime ?? DateTime.MinValue).TimeOfDay;
            var endDateTime = startDateTime.AddMinutes(shexieAppointment.AppointmentType.Duration);

            return new MigrationAppointment
            {
                ExternalId = shexieAppointment.Id.ToString(),
                Name = shexieAppointment.Description,
                Description = shexieAppointment.Description,
                CreationDate = shexieAppointment.CreationDate ?? DateTime.MinValue,
                UserExternalId = shexieAppointment.Provider,
                PatientExternalId = shexieAppointment.PatientId.ToString(),
                AppointmentTypeExternalId = shexieAppointment.AppointmentTypeId.ToString(),
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                CancellationReasonId = shexieAppointment.Status == 2 ? AppointmentCancelledByPatient : null
            };
        }

        internal static MigrationScript ToMigrationScript(this ShexieScript shexieScript)
        {
            var drug = shexieScript.Drug != null ? ToMigrationDrug(shexieScript.ShexieDrug) : null;

            return new MigrationScript
            {
                ExternalId = shexieScript.Id.ToString(),
                PatientExternalId = shexieScript.PatientId.ToString(),
                PrescriptionNo = shexieScript.PrescriptionNo,
                PrescriptionDose = shexieScript.PrescriptionDose,
                RepeatDays = shexieScript.RepeatDays,
                AuthorityNumber = shexieScript.AuthorityNumber,
                Duration = shexieScript.Duration,
                Created = shexieScript.Created.AsInvariantString(),
                RequestedDate = shexieScript.RequestedDate.AsInvariantString(),
                PrescriptionDate = shexieScript.PrescriptionDate.AsInvariantString(),
                PrescriptionRepeats = shexieScript.PrescriptionRepeats.ToString(),
                ProviderNumber = shexieScript.Provider,
                Dose = shexieScript.Dose,
                Substitute = shexieScript.Substitute.ToString(),
                ExternalDrugId = shexieScript.Drug.ToString(),
                Quantity = shexieScript.Quantity.ToString(),
                OtherMedicalDescription = shexieScript.OtherMedicalDescription,
                Frequency = shexieScript.Frequency.ToString(),
                Strength = shexieScript.Strength.ToString(),
                Delivery = shexieScript.Delivery.ToString(),
                Comments = shexieScript.Comments,
                ScriptWords = shexieScript.ScriptWords,
                AverageDailyDose = shexieScript.AverageDailyDose.ToString(),
                Indics = shexieScript.Indics,
                MigrationDrug = drug,
                ExternalUserId = shexieScript.Provider,
            };
        }

        internal static MigrationDrug ToMigrationDrug(this ShexieDrug shexieDrug)
        {
            return new MigrationDrug
            {
                ExternalId = shexieDrug.Id.ToString(),
                Name = shexieDrug.Name,
                Composition = shexieDrug.Composition,
                Strength = shexieDrug.Strength.ToString(),
            };
        }

        internal static MigrationDocument ToMigrationDocument(this ShexieAttachment shexieAttachment, string documentsPath)
        {
            var filepath = shexieAttachment.Path.GetDocumentFilepath(documentsPath);

            return new MigrationDocument
            {
                ExternalId = shexieAttachment.Id.ToString(),
                Description = shexieAttachment.Description,
                ImageDate = shexieAttachment.Date,
                PatientExternalId = shexieAttachment.PatientId,
                FileName = Path.GetFileName(shexieAttachment.Path),
                Md5 = filepath,
            };
        }

        internal static MigrationLetter ToMigrationLetter(this ShexieAttachment shexieAttachment, string documentsPath)
        {
            var filepath = shexieAttachment.Path.GetDocumentFilepath(documentsPath);

            return new MigrationLetter
            {
                ExternalId = shexieAttachment.Id.ToString(),
                AuthorExternalId = shexieAttachment.Provider,
                Date = shexieAttachment.Date.GetValueOrDefault(),
                PatientExternalId = shexieAttachment.PatientId,
                Text = filepath,
                Status = MigrationLetterStatus.Sent,
            };
        }

        internal static MigrationRecall ToMigrationRecall(this ShexieRecall shexieRecall)
        {
            var parsedName = !string.IsNullOrWhiteSpace(shexieRecall.ShexieProvider.ProviderName)
                ? shexieRecall.ShexieProvider.ProviderName.ParseName(SalutationShexieToSiberia.Keys.ToArray())
                : null;

            return new MigrationRecall
            {
                ExternalId = shexieRecall.Id.ToString(),
                PatientExternalId = shexieRecall.PatientId.ToString(),
                DoctorExternalId = shexieRecall.Provider,
                Reason = shexieRecall.Reason,
                IsCompleted = shexieRecall.IsCancel || shexieRecall.NoFollowUp == 1,
                PatientPhone = GetActivePhone(shexieRecall.ShexiePatient.MobilePhone, shexieRecall.ShexiePatient.HomePhone),
                DoctorFullName = parsedName != null ? ToFullName(parsedName.Salutation, parsedName.FirstName, parsedName.LastName) : null,
                RecurrenceInterval = shexieRecall.RecurrenceInterval,
                DateEntered = shexieRecall.Date.AddYears(-shexieRecall.Years).AddMonths(-shexieRecall.Months).AddDays(-((shexieRecall.Weeks * 7) + shexieRecall.Days)),
                NextCallDate = shexieRecall.Date,
                DueDate = shexieRecall.DueDate,
            };
        }

        internal static MigrationAllergy ToMigrationAllergy(IGrouping<int, string> shexieAllergy)
        {
            return new MigrationAllergy
            {
                Allergies = shexieAllergy.ToArray(),
                PatientExternalId = shexieAllergy.Key.ToString(),
            };
        }

        internal static MigrationInterestedParty ToMigrationInterestedParty(this ShexieInterestedParty shexieInterestedParty)
        {
            return new MigrationInterestedParty
            {
                ExternalId = shexieInterestedParty.Id.ToString(),
                ContactExternalId = shexieInterestedParty.ContactId.ToString(),
                PatientExternalId = shexieInterestedParty.PatientId.ToString(),
            };
        }

        internal static MigrationConsult ToMigrationConsult(this ShexiePatientHistory shexieHistory)
        {
            return new MigrationConsult
            {
                ExternalId = shexieHistory.Id.ToString(),
                StartConsult = shexieHistory.Date ?? DateTime.MinValue,
                History = RtfToPlainTextConverter.ConvertRtfToPlainText(shexieHistory.Note),
                PatientExternalId = shexieHistory.PatientId.ToString(),
            };
        }

        private static string ToFullName(string salutation, string firstName, string lastName)
        {
            var names = new[] { salutation, firstName, lastName };

            return string.Join(" ", names.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string GetActivePhone(string mobilePhone, string homePhone)
        {
            return !string.IsNullOrWhiteSpace(mobilePhone) ? mobilePhone : homePhone;
        }

        private static string GetDocumentFilepath(this string shexiePath, string documentsPath)
        {
            string localPath = shexiePath;
            if (!string.IsNullOrWhiteSpace(shexiePath) && !string.IsNullOrWhiteSpace(documentsPath))
            {
                string rootPath = Path.GetPathRoot(shexiePath);
                localPath = Path.Combine(documentsPath, shexiePath.Substring(rootPath.Length));
            }

            return localPath;
        }

        private static string FormatLabResultData(this ShexieLaboratoryResult shexieLabResult, string documentsPath)
        {
            switch ((ShexieLabResultCompressed)shexieLabResult.Compressed)
            {
                case ShexieLabResultCompressed.RtfText:
                    return shexieLabResult.Results.ReplaceRtfTagByHtmlBreakTag();

                case ShexieLabResultCompressed.HtmlFilePath:
                    var path = shexieLabResult.Results.GetDocumentFilepath(documentsPath);
                    return File.Exists(path) ? File.ReadAllText(path) : null;

                default:
                    return shexieLabResult.Results;
            }
        }

        private static string FormatNoteData(ShexiePatientHistory history)
        {
            switch ((ShexieHistoryNoteFormatted)history.Formatted)
            {
                case ShexieHistoryNoteFormatted.PlainTextForPatientNote:
                    return history.Note;

                case ShexieHistoryNoteFormatted.RtfTextForPatientNote:
                    return RtfToPlainTextConverter.ConvertRtfToPlainText(history.Note);

                default:
                    return string.Empty;
            }
        }
    }
}