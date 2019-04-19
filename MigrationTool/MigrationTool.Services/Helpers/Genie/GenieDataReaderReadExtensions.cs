using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using MigrationTool.Services.Entities.Genie;

namespace MigrationTool.Services.Helpers.Genie
{
    internal static class GenieDataReaderReadExtensions
    {
        internal static GenieUser GetGenieUser(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieUser
            {
                Id = reader.GetInt32(columns["Id"]),
                Title = reader.GetString(columns["Title"]),
                FirstName = reader.GetString(columns["FirstName"]),
                Surname = reader.GetString(columns["Surname"]),
                MiddleName = reader.GetString(columns["MiddleName"]),
                Name = reader.GetString(columns["Name"]),
                UserName = reader.GetString(columns["UserName"]),
                Specialty = reader.GetString(columns["Specialty"]),
            };
        }

        internal static int GetGenieUserCount(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return reader.GetInt32(columns["UserCount"]);
        }

        internal static GenieRecall GetGenieRecall(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieRecall
            {
                DateEntered = reader.GetNullableDateTime(columns["DateEntered"]),
                DoctorFullName = reader.GetString(columns["Doctor"]),
                DueDate = reader.GetNullableDateTime(columns["DueDate"]),
                Id = reader.GetInt32(columns["Id"]),
                IsCompleted = reader.GetBoolean(columns["Completed"]),
                LastActionDate = reader.GetNullableDateTime(columns["Last"]),
                LastActionDetails = reader.GetString(columns["ActionDetail"]),
                NextAppointmentDate = reader.GetNullableDateTime(columns["NextAppt"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                PatientPhone = reader.GetString(columns["PatientPhone"]),
                Reason = reader.GetString(columns["Reason"]),
                RecurrenceInterval = reader.GetInt16(columns["RecurInterval"])
            };
        }

        internal static GeniePatient GetGeniePatient(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GeniePatient
            {
                Id = reader.GetInt32(columns["Id"]),
                AccountHolderId = reader.GetInt32(columns["ah_id_fk"]),
                UsualGpId = reader.GetInt32(columns["UsualGP_AB_Id_Fk"]),
                UsualProvider = reader.GetString(columns["UsualProvider"]),
                FirstName = reader.GetString(columns["FirstName"]),
                Surname = reader.GetString(columns["Surname"]),
                KnownAs = reader.GetString(columns["KnownAs"]),
                Dob = reader.GetNullableDateTime(columns["DOB"]),
                Dod = reader.GetNullableDateTime(columns["DOD"]),
                Deceased = reader.GetBoolean(columns["Deceased"]),
                ChartNumber = reader.GetString(columns["ChartOrNHS"]),
                AddressLine1 = reader.GetString(columns["AddressLine1"]),
                Suburb = reader.GetString(columns["Suburb"]),
                PostCode = reader.GetString(columns["Postcode"]),
                Sex = reader.GetString(columns["Sex"]),
                HomePhone = reader.GetString(columns["HomePhone"]),
                SmokingInfo = reader.GetString(columns["SmokingInfo"]),
                SmokingFreq = reader.GetNullableInt16(columns["SmokingFreq"]),
                AlcoholInfo = reader.GetString(columns["AlcoholInfo"]),
                Alcohol = reader.GetBoolean(columns["Alcohol"]),
                Title = reader.GetString(columns["Title"]),
                WorkPhone = reader.GetString(columns["WorkPhone"]),
                MedicareNum = reader.GetString(columns["MedicareNum"]),
                MedicareRefNum = reader.GetInt16(columns["MedicareRefNum"]),
                State = reader.GetString(columns["State"]),
                EmailAddress = reader.GetString(columns["EmailAddress"]),
                MiddleName = reader.GetString(columns["MiddleName"]),
                HealthFundNumber = reader.GetString(columns["HealthFundNum"]),
                HealthFundName = reader.GetString(columns["HealthFundName"]),
                CountryOfBirth = reader.GetString(columns["CountryOfBirth"]),
                Language = reader.GetString(columns["Language"]),
                AddressLine2 = reader.GetString(columns["AddressLine2"]),
                MaritalStatus = reader.GetString(columns["MaritalStatus"]),
                MobilePhone = reader.GetString(columns["MobilePhone"]),
                MedicareExpiry = reader.GetNullableDateTime(columns["MedicareExpiry"]),
                Country = reader.GetString(columns["Country"]),
                PartnerName = reader.GetString(columns["PartnerName"]),
                MaidenName = reader.GetString(columns["MaidenName"]),
                AccountType = reader.GetString(columns["AccountType"]),
                DoNotSendSms = reader.GetBoolean(columns["DontSms"]),
                DvaNumber = reader.GetString(columns["DvaNum"]),
                DvaDisability = reader.GetString(columns["DVADisability"]),
                DvaCardColour = reader.GetString(columns["DVACardColour"]),
                HccPensionNumber = reader.GetString(columns["HccPensionNum"]),
                HccPensionExpiry = reader.GetNullableDateTime(columns["HCCExpiry"]),
                Scratchpad = reader.GetString(columns["Scratchpad"]),
                NokName = reader.GetString(columns["NokName"]),
                NokPhone = reader.GetString(columns["NokPhone"]),
                Memo = reader.GetString(columns["Memo"]),
                ExternalId = reader.GetString(columns["ExternalId"]),
            };
        }

        internal static GenieConsultProblem GetGenieConsultProblem(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieConsultProblem
            {
                Id = reader.GetInt32(columns["Id"]),
                ConsultId = reader.GetInt32(columns["CNSLT_Id_Fk"]),
                Problem = reader.GetString(columns["Problem"]),
                IsPrimaryProblem = reader.GetBoolean(columns["IsPrimaryProblem"]),
            };
        }

        internal static GenieCurrentProblem GetGenieCurrentProblem(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieCurrentProblem
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                Problem = reader.GetString(columns["Problem"]),
                Note = reader.GetString(columns["Note"]),
                DiagnosisDate = reader.GetNullableDateTime(columns["DiagnosisDate"]),
                Confidential = reader.GetBoolean(columns["Confidential"]),
            };
        }

        internal static GeniePastHistory GetGeniePastHistory(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GeniePastHistory
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                History = reader.GetString(columns["History"]),
                Note = reader.GetString(columns["Note"]),
                CreationDate = reader.GetNullableDateTime(columns["CreationDate"]),
                Confidential = reader.GetBoolean(columns["Confidential"]),
            };
        }

        internal static GenieAppointment GetGenieAppointment(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieAppointment
            {
                Id = reader.GetInt32(columns["Id"]),
                CreatedBy = reader.GetString(columns["CreatedBy"]),
                CreationDate = reader.GetNullableDateTime(columns["CreationDate"]),
                Note = reader.GetString(columns["Note"]),
                StartDate = reader.GetNullableDateTime(columns["StartDate"]),
                StartTime = reader.GetNullableTimeSpan(columns["StartTime"]),
                ProviderId = reader.GetInt32(columns["ProviderId"]),
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
                Name = reader.GetString(columns["Name"]),
                Reason = reader.GetString(columns["Reason"]),
                Duration = reader.GetInt32(columns["ApptDuration"]),
            };
        }

        internal static GenieAppointmentType GetGenieAppointmentTypes(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieAppointmentType
            {
                Id = reader.GetInt32(columns["Id"]),
                Note = reader.GetString(columns["Note"]),
                Colour = reader.GetInt32(columns["Colour"]),
                Duration = reader.GetInt32(columns["Duration"]),
            };
        }

        internal static GenieContact GetGenieContact(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieContact
            {
                Address1 = reader.GetString(columns["Address1"]),
                Address2 = reader.GetString(columns["Address2"]),
                AllTalk = reader.GetString(columns["AllTalk"]),
                Argus = reader.GetString(columns["Argus"]),
                Category = reader.GetString(columns["Category"]),
                Clinic = reader.GetString(columns["Clinic"]),
                Country = reader.GetString(columns["Country"]),
                DivisionReport = reader.GetString(columns["DivisionReport"]),
                EmailAddress = reader.GetString(columns["EmailAddress"]),
                Fax = reader.GetString(columns["Fax"]),
                FirstName = reader.GetString(columns["FirstName"]),
                HealthLink = reader.GetString(columns["HealthLink"]),
                Homephone = reader.GetString(columns["Homephone"]),
                Hpii = reader.GetString(columns["Hpii"]),
                Id = reader.GetInt32(columns["Id"]),
                Initial = reader.GetString(columns["Initial"]),
                MedicalObject = reader.GetString(columns["MedicalObject"]),
                Mobile = reader.GetString(columns["Mobile"]),
                PostCode = reader.GetString(columns["PostCode"]),
                ProviderNum = reader.GetString(columns["ProviderNum"]),
                ReferralNet = reader.GetString(columns["ReferralNet"]),
                IsAssistant = reader.GetNullableBoolean(columns["Assists"]),
                Specialty = reader.GetString(columns["Specialty"]),
                State = reader.GetString(columns["State"]),
                Suburb = reader.GetString(columns["Suburb"]),
                Surname = reader.GetString(columns["Surname"]),
                FullName = reader.GetString(columns["FullName"]),
                Title = reader.GetString(columns["Title"]),
                WorkPhone = reader.GetString(columns["WorkPhone"]),
            };
        }

        internal static GenieReferral GetGenieReferral(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieReferral
            {
                ContactId = reader.GetInt32(columns["ab_id_fk"]),
                Duration = reader.GetInt16(columns["duration"]),
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
                ReferralDate = reader.GetNullableDateTime(columns["ReferralDate"]),
                IssueDate = reader.GetNullableDateTime(columns["IssueDate"]),
                ExpiryDate = reader.GetNullableDateTime(columns["ExpiryDate"]),
                ReferredTo = reader.GetString(columns["ReferredTo"]),
            };
        }

        internal static GenieConsult GetGenieConsult(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieConsult
            {
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
                ConsultDate = reader.GetNullableDateTime(columns["ConsultDate"]),
                History = reader.GetString(columns["History"]),
                DoctorId = reader.GetInt32(columns["DoctorId"]),
                ConsultTime = reader.GetTimeSpan(columns["ConsultTime"]),
                Diagnosis = reader.GetString(columns["Diagnosis"]),
                Plan = reader.GetString(columns["Plan"]),
                Id = reader.GetInt32(columns["Id"]),
                Examination = reader.GetString(columns["Examination"]),
            };
        }

        internal static GenieAllergy GetGenieAllergy(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieAllergy
            {
                Allergy = reader.GetString(columns["Allergy"]),
                Detail = reader.GetString(columns["Detail"]),
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
            };
        }

        internal static GenieMeasurement GetGenieMeasurement(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieMeasurement
            {
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
                MeasurementDate = reader.GetNullableDateTime(columns["MeasurementDate"]),
                Height = reader.GetFloat(columns["Height"]),
                Weight = reader.GetFloat(columns["Weight"]),
                HeadCircumference = reader.GetFloat(columns["HeadCircumference"]),
                Waist = reader.GetFloat(columns["Waist"]),
                Bmi = reader.GetFloat(columns["Bmi"]),
                Hip = reader.GetFloat(columns["Hip"]),
                HeartRate = reader.GetFloat(columns["HeartRate"]),
                Id = reader.GetInt32(columns["Id"]),
                Diastolic = reader.GetFloat(columns["Diastolic"]),
                Systolic = reader.GetFloat(columns["Systolic"]),
            };
        }

        internal static GenieDownloadedResult GetGenieDownloadedResult(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieDownloadedResult
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
                Addressee = reader.GetString(columns["Addressee"]),
                DocumentName = reader.GetString(columns["DocumentName"]),
                DateOfBirth = reader.GetNullableDateTime(columns["Dob"]),
                ReportDate = reader.GetNullableDateTime(columns["ReportDate"]),
                ImportDate = reader.GetNullableDateTime(columns["ImportDate"]),
                CollectionDate = reader.GetNullableDateTime(columns["CollectionDate"]),
                ReceivedDate = GenieMigrationHelper.ParseRecivedDateString(reader.GetString(columns["ReceivedDate"])),
                FirstName = reader.GetString(columns["FirstName"]),
                Surname = reader.GetString(columns["Surname"]),
                Result = reader.GetString(columns["Result"]),
                Test = reader.GetString(columns["Test"]),
                LabRef = reader.GetString(columns["LabRef"]),
                NormalOrAbnormal = reader.GetString(columns["NormalOrAbnormal"]),
            };
        }

        internal static GenieGraphic GetGenieGraphic(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieGraphic
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["pt_id_fk"]),
                PathName = reader.GetString(columns["PathName"]),
                RealName = reader.GetString(columns["RealName"]),
                FirstName = reader.GetString(columns["FirstName"]),
                Surname = reader.GetString(columns["Surname"]),
                ImageDate = reader.GetNullableDateTime(columns["ImageDate"]),
                Description = reader.GetString(columns["Description"]),
            };
        }

        internal static GenieAccountHolder GetGenieAccountHolder(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieAccountHolder
            {
                AddressLine1 = reader.GetString(columns["AddressLine1"]),
                AddressLine2 = reader.GetString(columns["AddressLine2"]),
                DateOfBirth = reader.GetNullableDateTime(columns["DOB"]),
                FirstName = reader.GetString(columns["FirstName"]),
                HomePhone = reader.GetString(columns["HomePhone"]),
                FullName = reader.GetString(columns["FullName"]),
                Fax = reader.GetString(columns["Fax"]),
                Mobile = reader.GetString(columns["Mobile"]),
                Id = reader.GetInt32(columns["Id"]),
                Individual = reader.GetBoolean(columns["Individual"]),
                MedicareExpiry = reader.GetNullableDateTime(columns["MedicareExpiry"]),
                MedicareNum = reader.GetString(columns["MedicareNum"]),
                MedicareRefNum = reader.GetInt16(columns["MedicareRefNum"]),
                Organisation = reader.GetString(columns["Organisation"]),
                PostCode = reader.GetString(columns["Postcode"]),
                State = reader.GetString(columns["State"]),
                Suburb = reader.GetString(columns["Suburb"]),
                Surname = reader.GetString(columns["Surname"]),
                Title = reader.GetString(columns["Title"]),
            };
        }

        internal static GenieTask GetGenieTask(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieTask
            {
                ContactId = reader.GetInt32(columns["AB_Id_Fk"]),
                Completed = reader.GetBoolean(columns["Completed"]),
                Creator = reader.GetString(columns["Creator"]),
                DateCompleted = reader.GetNullableDateTime(columns["DateCompleted"]),
                DateCreated = reader.GetNullableDateTime(columns["DateCreated"]),
                Id = reader.GetInt32(columns["Id"]),
                LastUpdatedBy = reader.GetNullableInt32(columns["LastUpdatedBy"]),
                Note = reader.GetString(columns["Note"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                Read = reader.GetBoolean(columns["Read"]),
                Sync = reader.GetBoolean(columns["Sync"]),
                Task = reader.GetString(columns["Task"]),
                TaskDate = reader.GetNullableDateTime(columns["TaskDate"]),
                TaskFor = reader.GetString(columns["TaskFor"]),
                TaskTime = reader.GetTimeSpan(columns["TaskTime"]),
                UrgentFg = reader.GetBoolean(columns["UrgentFg"]),
                Version = reader.GetInt32(columns["Version"]),
            };
        }

        internal static GenieChecklist GetGenieChecklist(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieChecklist
            {
                DateCreated = reader.GetNullableDateTime(columns["DateCreated"]),
                Id = reader.GetInt32(columns["Id"]),
                Name = reader.GetString(columns["Name"]),
                PRCDRE_Id_Fk = reader.GetInt32(columns["PRCDRE_Id_Fk"]),
                Provider = reader.GetString(columns["Provider"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
            };
        }

        internal static GenieChecklistField GetGenieChecklistField(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieChecklistField
            {
                Id = reader.GetInt32(columns["Id"]),
                ChecklistId = reader.GetInt32(columns["CHKLST_Id_Fk"]),
                ChecklistFieldId = reader.GetInt32(columns["CHKLSTFD_Id_Fk"]),
                Type = reader.GetString(columns["Type"]),
                Position = reader.GetInt32(columns["Position"]),
                Label = reader.GetString(columns["Label"]),
                FieldData = Encoding.UTF8.GetString(reader.GetBlob(columns["FieldData"])),
            };
        }

        internal static GenieVaccination GetGenieVaccination(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieVaccination
            {
                ACIRCode = reader.GetString(columns["ACIRCode"]),
                Dose = reader.GetInt16(columns["DoseNum"]),
                ExpiryDate = reader.GetNullableDateTime(columns["ExpiryDate"]),
                GivenDate = reader.GetNullableDateTime(columns["GivenDate"]),
                ICD10Code = reader.GetString(columns["ICD10Code"]),
                ICPCCode = reader.GetString(columns["ICPCCode"]),
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                TermCode = reader.GetString(columns["TermCode"]),
                Vaccine = reader.GetString(columns["Vaccine"]),
            };
        }

        internal static GenieOpReport GetGenieOpReport(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieOpReport
            {
                Id = reader.GetInt32(columns["Id"]),
                DoctorId = reader.GetInt32(columns["ProviderKey"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                AssistantId = reader.GetInt32(columns["Assistant_AB_Id_Fk"]),
                AnaesthetistId = reader.GetInt32(columns["Anaesthetist_AB_Id_Fk"]),
                ProcedureName = reader.GetString(columns["ProcedureName"]),
                Side = reader.GetNullableInt32(columns["Side"]),
                DoctorName = reader.GetString(columns["Provider"]),
                Hospital = reader.GetString(columns["Hospital"]),
                ProcedureDate = reader.GetNullableDateTime(columns["ProcedureDate"]),
                ProcedureTimeFrom = reader.GetNullableTimeSpan(columns["StartTime"]),
                ProcedureTimeTo = reader.GetNullableTimeSpan(columns["EndTime"]),
                AdmissionDate = reader.GetNullableDateTime(columns["AdmissionDate"]),
                AdmissionTime = reader.GetNullableTimeSpan(columns["AdmissionTime"]),
                FastFromTime = reader.GetNullableTimeSpan(columns["FastingFrom"]),
                DischargeDate = reader.GetNullableDateTime(columns["DischargeDate"]),
                InPatientDays = reader.GetNullableInt16(columns["DaysHospitalised"]),
                Indication = reader.GetString(columns["ClinicalIndication"]),
                Category = reader.GetString(columns["Category"]),
                Magnitude = reader.GetString(columns["Magnitude"]),
                InfectionRisk = reader.GetString(columns["InfectionRisk"]),
                ProcedureType = reader.GetString(columns["ProcedureType"]),
                Anaesthetic = reader.GetString(columns["Anaesthetic"]),
                Prosthesis = reader.GetString(columns["Prosthesis"]),
                Finding = reader.GetString(columns["Finding"]),
                Technique = reader.GetString(columns["Technique"]),
                PostOp = reader.GetString(columns["PostOp"]),
                AdmissionOutcome = reader.GetString(columns["AdmissionOutcome"]),
                FollowupDate = reader.GetNullableDateTime(columns["FollowupDate"]),
                FollowupOutcome = reader.GetString(columns["FollowupOutcome"]),
                AuditSummary = reader.GetString(columns["AuditSummary"]),
                PreopDiagnosis = reader.GetString(columns["PreopDiagnosis"]),
                PostopDiagnosis = reader.GetString(columns["PostopDiagnosis"]),
            };
        }

        internal static GenieQuoteItem GetGenieQuoteItem(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieQuoteItem
            {
                Id = reader.GetInt32(columns["Id"]),
                ItemNumber = reader.GetString(columns["ItemNum"]),
                Description = reader.GetString(columns["Description"]),
                Number = reader.GetInt16(columns["Num"]),
                Rebate = reader.GetFloat(columns["Rebate"]),
                Fraction = reader.GetFloat(columns["Fraction"]),
                Fee = reader.GetFloat(columns["Fee"]),
                Gst = reader.GetFloat(columns["GST"]),
                AssistantBillable = reader.GetBoolean(columns["AssistantBillable"]),
                KnownGap = reader.GetFloat(columns["KnownGap"]),
                AmaFee = reader.GetFloat(columns["AMAFee"]),
                OpReportId = reader.GetInt32(columns["PRCDRE_Id_Fk"]),
            };
        }

        internal static GenieComplication GetGenieComplication(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieComplication
            {
                Id = reader.GetInt32(columns["Id"]),
                ProcedureId = reader.GetInt32(columns["PRCDRE_Id_Fk"]),
                ComplicationDate = reader.GetNullableDateTime(columns["ComplicationDate"]),
                ComplicationDetails = reader.GetString(columns["Complication"]),
            };
        }

        internal static GenieDrug GetGenieDrug(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieDrug
            {
                Id = reader.GetString(columns["UniqueCode"]),
                Form = reader.GetString(columns["Form"]),
                Name = reader.GetString(columns["Drug"]),
                Quantity = reader.GetString(columns["Quantity"]),
                Strength = reader.GetString(columns["Strength"]),
                Code = reader.GetString(columns["Code"]),
                MimsProdCode = reader.GetNullableInt16(columns["ProdCode"]),
                MimsFormCode = reader.GetNullableInt16(columns["FormCode"]),
                MimsPackCode = reader.GetNullableInt16(columns["PackCode"]),
            };
        }

        internal static GenieScript GetGenieScript(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieScript
            {
                Id = reader.GetString(columns["Id"]),
                CreationDate = reader.GetNullableDateTime(columns["CreationDate"]),
                Dose = reader.GetString(columns["Dose"]),
                ScriptNumber = reader.GetInt32(columns["ScriptNum"]),
                ApprovalNumber = reader.GetString(columns["ApprovalNum"]),
                PatientId = reader.GetInt32(columns["PT_Id_fk"]),
                Quantity = reader.GetString(columns["Qty"]),
                DrugId = reader.GetString(columns["DrugIndexCode"]),
                Note = reader.GetString(columns["Note"]),
                AuthorityNumber = reader.GetString(columns["AuthorityNum"]),
                Medication = reader.GetString(columns["Medication"]),
                Repeat = reader.GetString(columns["Repeat"]),
                CreatorDoctorId = reader.GetNullableInt32(columns["DoctorId"]),
            };
        }

        internal static GenieAntenatalVisit GetGenieAntenatalVisit(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieAntenatalVisit
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                PregnancyId = reader.GetInt32(columns["PRG_Id_Fk"]),
                VisitDate = reader.GetNullableDateTime(columns["VisitDate"]),
                UserName = reader.GetString(columns["RecordedBy"]),
                Gestation = reader.GetString(columns["Gestation"]),
                Fundus = reader.GetInt16(columns["Fundus"]),
                Weight = reader.GetFloat(columns["Weight"]),
                Urine = reader.GetString(columns["Urine"]),
                Bp = reader.GetString(columns["BP"]),
                LiquorVolume = reader.GetString(columns["LiquorVolume"]),
                Oedema = reader.GetString(columns["Oedema"]),
                Note = reader.GetString(columns["Note"]),
                Presentation = reader.GetString(columns["Presentation"]),
                Station = reader.GetString(columns["Station"]),
                Fm = reader.GetString(columns["FM"]),
                Fh = reader.GetString(columns["FH"]),
            };
        }

        internal static GenieObstetricHistory GetGenieObstetricHistory(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieObstetricHistory
            {
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                DeliveryYear = reader.GetInt32(columns["DeliveryYear"]),
                Place = reader.GetString(columns["Place"]),
                Week = reader.GetString(columns["Week"]),
                Pregnancy = reader.GetString(columns["Pregnancy"]),
                Labour = reader.GetString(columns["Labour"]),
                Analgesia = reader.GetString(columns["Analgesia"]),
                Delivery = reader.GetString(columns["Delivery"]),
                Sex = reader.GetString(columns["Sex"]),
                Weight = reader.GetFloat(columns["Weight"]),
                Name = reader.GetString(columns["Name"]),
                Note = reader.GetString(columns["Note"]),
                Id = reader.GetInt32(columns["Id"]),
                Result = reader.GetString(columns["Result"]),
                BreastFed = reader.GetString(columns["BreastFed"]),
                Sex2 = reader.GetString(columns["Sex2"]),
                Weight2 = reader.GetFloat(columns["Weight2"]),
                Name2 = reader.GetString(columns["Name2"]),
                Result2 = reader.GetString(columns["Result2"]),
                Sex3 = reader.GetString(columns["Sex3"]),
                Weight3 = reader.GetFloat(columns["Weight3"]),
                Name3 = reader.GetString(columns["Name3"]),
                Result3 = reader.GetString(columns["Result3"]),
                Sex4 = reader.GetString(columns["Sex4"]),
                Weight4 = reader.GetFloat(columns["Weight4"]),
                Name4 = reader.GetString(columns["Name4"]),
                Result4 = reader.GetString(columns["Result4"]),
                DeliveryDate = reader.GetNullableDateTime(columns["DeliveryDate"]),
                Induction = reader.GetString(columns["Induction"]),
            };
        }

        internal static GeniePregnancy GetGeniePregnancy(DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GeniePregnancy
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                Lmp = reader.GetNullableDateTime(columns["LMP"]),
                Edd = reader.GetNullableDateTime(columns["EDD"]),
                Problems = reader.GetString(columns["Problems"]),
                Cvs = reader.GetString(columns["ChorVillSampling"]),
                Amnio = reader.GetString(columns["Amnio"]),
                Chest = reader.GetString(columns["Chest"]),
                Breasts = reader.GetString(columns["Breasts"]),
                Abdo = reader.GetString(columns["Abdo"]),
                Pv = reader.GetString(columns["PV"]),
                Pap = reader.GetString(columns["Pap"]),
                BloodGroup = reader.GetString(columns["BloodGroup"]),
                Hb = reader.GetFloat(columns["Hb"]),
                Mcv = reader.GetFloat(columns["MCV"]),
                Rubella = reader.GetString(columns["Rubella"]),
                Hbv = reader.GetString(columns["HBV"]),
                Hcv = reader.GetString(columns["HCV"]),
                Hiv = reader.GetString(columns["HIV"]),
                Syphilis = reader.GetString(columns["Syphilis"]),
                Msu = reader.GetString(columns["MSU"]),
                Parvo = reader.GetString(columns["Parvo"]),
                Toxo = reader.GetString(columns["Toxo"]),
                Cmv = reader.GetString(columns["CMV"]),
                Varicella = reader.GetString(columns["Varicella"]),
                Uss1Weeks = reader.GetFloat(columns["USS1Weeks"]),
                Uss1Comment = reader.GetString(columns["USS1Comment"]),
                Uss2Weeks = reader.GetFloat(columns["USS2Wks"]),
                Uss2Comment = reader.GetString(columns["USS2Comment"]),
                Uss3Weeks = reader.GetFloat(columns["USS3Wks"]),
                Uss3Comment = reader.GetString(columns["USS3Comment"]),
                Hb28Weeks = reader.GetFloat(columns["Hb28Wks"]),
                Hb36Weeks = reader.GetFloat(columns["Hb36Wks"]),
                Gct28Weeks = reader.GetString(columns["GCT28Wks"]),
                Antibodies28Weeks = reader.GetString(columns["Antibodies28Wks"]),
                Antibodies36Weeks = reader.GetString(columns["Antibodies36Wks"]),
                Gbs28Weeks = reader.GetString(columns["GBS28Wks"]),
                Gbs36Weeks = reader.GetString(columns["GBS36Wks"]),
                Notes = reader.GetString(columns["Notes"]),
                Declined = reader.GetBoolean(columns["Declined"]),
                DeliveryDate = reader.GetNullableDateTime(columns["DeliveryDate"]),
                DeliveryTime = reader.GetNullableTimeSpan(columns["DeliveryTime"]),
                DeliveryMethod = reader.GetString(columns["DeliveryMethod"]),
                DeliveryNotes = reader.GetString(columns["DeliveryNotes"]),
                Accoucher = reader.GetString(columns["Accoucher"]),
                Paediatrician = reader.GetString(columns["Paediatrician"]),
                Anaesthetist = reader.GetString(columns["Anaesthetist"]),
                LabourNil = reader.GetBoolean(columns["LabourNil"]),
                AnalgesiaNil = reader.GetBoolean(columns["AnalgesiaNil"]),
                Perineum = reader.GetString(columns["Perineum"]),
                BabySex = reader.GetString(columns["BabySex"]),
                BabyWeight = reader.GetFloat(columns["BabyWt"]),
                Apgar1 = reader.GetNullableInt16(columns["Apgar1"]),
                Apgar2 = reader.GetNullableInt16(columns["Apgar5"]),
                BabyName = reader.GetString(columns["BabyName"]),
                PostnatalDate = reader.GetNullableDateTime(columns["PostnatalDate"]),
                Breastfeeding = reader.GetBoolean(columns["Breastfeeding"]),
                Lochia = reader.GetInt16(columns["LochiaNo"]),
                PerineumState = reader.GetInt16(columns["PerineumNo"]),
                Bladder = reader.GetInt16(columns["BladderNo"]),
                Bowel = reader.GetInt16(columns["BowelNo"]),
                BreastState = reader.GetInt16(columns["BreastsNo"]),
                PnBreasts = reader.GetInt16(columns["PNBreastsNo"]),
                PnAbdo = reader.GetInt16(columns["PNAbdoNo"]),
                PnPerineum = reader.GetInt16(columns["PNPerineumNo"]),
                Speculum = reader.GetString(columns["Speculum"]),
                PostnatalPv = reader.GetString(columns["PostnatalPV"]),
                PapTaken = reader.GetBoolean(columns["PapTaken"]),
                Contraception = reader.GetString(columns["Contraception"]),
                PostnatalNotes = reader.GetString(columns["PostnatalNotes"]),
                ObstetricHistoryRecordId = reader.GetInt32(columns["OBSHX_Id_Fk"]),
                LabourSpontaneous = reader.GetBoolean(columns["LabourSpontaneous"]),
                LabourProstin = reader.GetBoolean(columns["LabourProstin"]),
                LabourArm = reader.GetBoolean(columns["LabourProstin"]),
                LabourSyntocinon = reader.GetBoolean(columns["LabourSyntocinon"]),
                AnalgesiaNitrous = reader.GetBoolean(columns["AnalgesiaNitrous"]),
                AnalgesiaPethidine = reader.GetBoolean(columns["AnalgesiaPethidine"]),
                AnalgesiaEpidural = reader.GetBoolean(columns["AnalgesiaEpidural"]),
                AnalgesiaSpinal = reader.GetBoolean(columns["AnalgesiaSpinal"]),
                AnalgesiaGa = reader.GetBoolean(columns["AnalgesiaGA"]),
                CurrentGestation = reader.GetString(columns["CurrentGestation"]),
                Uss1Date = reader.GetNullableDateTime(columns["USS1Date"]),
                Uss2Date = reader.GetNullableDateTime(columns["USS2Date"]),
                Uss3Date = reader.GetNullableDateTime(columns["USS3Date"]),
                AbsInitial = reader.GetString(columns["ABsInitial"]),
                EddAgreed = reader.GetNullableDateTime(columns["EDDAgreed"]),
                Placenta = reader.GetString(columns["Placenta"]),
                AntiD = reader.GetString(columns["AntiD"]),
                BabySex2 = reader.GetString(columns["BabySex2"]),
                BabyWeight2 = reader.GetFloat(columns["BabyWt2"]),
                Baby2Apgar1 = reader.GetNullableInt16(columns["Baby2Agpar1"]),
                Baby2Apgar2 = reader.GetNullableInt16(columns["Baby2Agpar2"]),
                BabyName2 = reader.GetString(columns["BabyName2"]),
                Hospital = reader.GetString(columns["Hospital"]),
                Provider = reader.GetString(columns["Provider"]),
                BoyOrGirl = reader.GetString(columns["BoyOrGirl"]),
                Ferritin = reader.GetString(columns["Ferritin"]),
                Tfts = reader.GetString(columns["TFTs"]),
                AntiD28 = reader.GetBoolean(columns["AntiD28"]),
                AntiD36 = reader.GetBoolean(columns["AntiD36"]),
                Gtt282Hr = reader.GetString(columns["GTT282Hr"]),
                TwentyWeekFee = reader.GetFloat(columns["TwentyWeekFee"]),
                ThirtyWeekFee = reader.GetFloat(columns["ThirtyWeekFee"]),
                BreastfeedingAtDelivery = reader.GetBoolean(columns["BreastfeedingAtDelivery"]),
                Result = reader.GetString(columns["Result"]),
                AdditionalAnNotes = reader.GetString(columns["AdditionalANNotes"]),
                Bp = reader.GetString(columns["BP"]),
                PlacentalPosition = reader.GetString(columns["PlacentalPosition"]),
                VitaminD = reader.GetFloat(columns["VitaminD"]),
                Trisomy18 = reader.GetString(columns["Trisomy18"]),
                Trisomy21 = reader.GetString(columns["Trisomy21"]),
                Ebl = reader.GetFloat(columns["EBL"]),
                Gtt28Fasting = reader.GetString(columns["GTT28Fasting"]),
                BabySex3 = reader.GetString(columns["BabySex3"]),
                BabySex4 = reader.GetString(columns["BabySex4"]),
                BabyWeight3 = reader.GetFloat(columns["BabyWt3"]),
                NextAppointmentDate = reader.GetNullableDateTime(columns["NextAppt"]),
                BabyWeight4 = reader.GetFloat(columns["BabyWt3"]),
                Baby3Apgar1 = reader.GetNullableInt16(columns["Baby3Apgar1"]),
                Baby3Apgar2 = reader.GetNullableInt16(columns["Baby3Apgar2"]),
                Baby4Apgar1 = reader.GetNullableInt16(columns["Baby4Apgar1"]),
                Baby4Apgar2 = reader.GetNullableInt16(columns["Baby4Apgar2"]),
                BabyName3 = reader.GetString(columns["BabyName3"]),
                BabyName4 = reader.GetString(columns["BabyName4"]),
                Result2 = reader.GetString(columns["Result2"]),
                Result3 = reader.GetString(columns["Result3"]),
                Result4 = reader.GetString(columns["Result4"]),
                AnaesthetistId = reader.GetNullableInt32(columns["Anaesthetist_AB_Id_Fk"]),
                PaediatricianId = reader.GetNullableInt32(columns["Paediatrician_AB_Id_Fk"]),
                AccoucherId = reader.GetNullableInt32(columns["Accoucher_AB_Id_Fk"]),
                Platelets28 = reader.GetString(columns["Platelets28"]),
                Platelets36 = reader.GetString(columns["Platelets36"]),
                InitialTestDate = reader.GetNullableDateTime(columns["InitialTestsDate"]),

                // [EG]: Actually columns["NeonatalNo"] in (0, 1, 2), will leave it as boolean for now
                NeonatalExam = reader.GetBoolean(columns["NeonatalExamBy"])
                    ? reader.GetInt16(columns["NeonatalNo"]) < 2
                    : (bool?)null
            };
        }

        internal static GenieInterestedParty GetGenieInterestedParties(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieInterestedParty
            {
                Id = reader.GetInt32(columns["Id"]),
                ContactId = reader.GetInt32(columns["AB_Id_Fk"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
            };
        }

        internal static GenieEmployer GetGenieEmployer(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieEmployer
            {
                Id = reader.GetString(columns["Id"]),
                Name = reader.GetString(columns["Name"]),
                AddressLine1 = reader.GetString(columns["AddressLine1"]),
                AddressLine2 = reader.GetString(columns["AddressLine2"]),
                Suburb = reader.GetString(columns["Suburb"]),
                State = reader.GetString(columns["State"]),
                PostCode = reader.GetString(columns["Postcode"]),
                Phone1 = reader.GetString(columns["Phone1"]),
                Phone2 = reader.GetString(columns["Phone2"]),
                Fax = reader.GetString(columns["Fax"]),
                Email = reader.GetString(columns["Email"]),
                Country = reader.GetString(columns["Country"]),
                InsurerId = reader.GetString(columns["Insurer_AH_Id_Fk"]),
            };
        }

        internal static GenieWorkCoverClaim GetGenieWorkCoverClaim(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieWorkCoverClaim
            {
                Id = reader.GetString(columns["Id"]),
                PatientId = reader.GetString(columns["PT_Id_Fk"]),
                ClaimNum = reader.GetString(columns["ClaimNum"]),
                InjuryDate = reader.GetNullableDateTime(columns["InjuryDate"]),
                Injury = reader.GetString(columns["Injury"]),
                EmployerId = reader.GetString(columns["EMPL_Id_Fk"]),
                InjuryTime = reader.GetNullableTimeSpan(columns["InjuryTime"]),
                InjuryMechanism = reader.GetString(columns["InjuryMechanism"]),
                Location = reader.GetString(columns["Location"]),
                CaseManagerName = reader.GetString(columns["FullName"]),
                CaseManagerWorkPhone = reader.GetString(columns["WorkPhone"]),
                CaseManagerMobilePhone = reader.GetString(columns["Mobile"]),
            };
        }

        internal static GenieDocument GetGenieDocument(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return new GenieDocument
            {
                Id = reader.GetInt32(columns["Id"]),
                PatientId = reader.GetInt32(columns["PT_Id_Fk"]),
                IsPrimary = reader.GetBoolean(columns["Primary"]),
                DocumentDate = reader.GetNullableDateTime(columns["DocumentDate"]),
                DateModified = reader.GetNullableDateTime(columns["DateModified"]),
                Title = reader.GetString(columns["Title"]),
                Note = reader.GetString(columns["Note"]),
            };
        }
    }
}