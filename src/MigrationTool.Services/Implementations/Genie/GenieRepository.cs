using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using MigrationTool.Services.Entities.Genie;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Helpers.Genie;
using MigrationTool.Services.Interfaces.Genie;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations.Genie
{
    internal class GenieRepository : IGenieRepository, IGenieConnectionTestService
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static readonly DbProviderFactory GenieDbProvider = DbProviderFactories.GetFactory("System.Data.Odbc");

        private static readonly string[] AllFields = { "*" };

        private static readonly string[] UserFields =
        {
            "Id",
            "Title",
            "FirstName",
            "Surname",
            "MiddleName",
            "Name",
            "UserName",
            "Specialty"
        };

        private static readonly string[] PatientFields =
        {
            "AccountType",
            "AddressLine1",
            "AddressLine2",
            "ah_id_fk",
            "AlcoholInfo",
            "Alcohol",
            "ChartOrNHS",
            "Country",
            "CountryOfBirth",
            "DOB",
            "DVACardColour",
            "DVADisability",
            "DontSms",
            "DvaNum",
            "EmailAddress",
            "FirstName",
            "HCCExpiry",
            "HccPensionNum",
            "HealthFundName",
            "HealthFundNum",
            "HomePhone",
            "Id",
            "Language",
            "MaidenName",
            "MaritalStatus",
            "MedicareExpiry",
            "MedicareNum",
            "MedicareRefNum",
            "MiddleName",
            "MobilePhone",
            "PartnerName",
            "Postcode",
            "Sex",
            "SmokingInfo",
            "SmokingFreq",
            "State",
            "Suburb",
            "Surname",
            "Title",
            "UsualGP_AB_Id_Fk",
            "UsualProvider",
            "WorkPhone",
            "Scratchpad",
            "Deceased",
            "DOD",
            "KnownAs",
            "NokName",
            "NokPhone",
            "Memo",
            "ExternalId"
        };

        private static readonly string[] ConsultationProblemFields =
        {
            "Id",
            "CNSLT_Id_Fk",
            "Problem",
            "IsPrimaryProblem"
        };

        private static readonly string[] CurrentProblemFields =
        {
            "Id",
            "PT_Id_Fk",
            "Problem",
            "Note",
            "DiagnosisDate",
            "Confidential"
        };

        private static readonly string[] PastHistoryFields =
        {
            "Id",
            "PT_Id_Fk",
            "History",
            "Note",
            "CreationDate",
            "Confidential"
        };

        private static readonly string[] AppointmentFields =
        {
            "Id",
            "CreatedBy",
            "CreationDate",
            "Note",
            "StartDate",
            "StartTime",
            "ProviderId",
            "pt_id_fk",
            "Name",
            "Reason",
            "ApptDuration"
        };

        private static readonly string[] ContactFields =
        {
            "Address1",
            "Address2",
            "AllTalk",
            "Argus",
            "Category",
            "Clinic",
            "Country",
            "DivisionReport",
            "EmailAddress",
            "Fax",
            "FirstName",
            "HealthLink",
            "Homephone",
            "Hpii",
            "Id",
            "Initial",
            "MedicalObject",
            "Mobile",
            "PostCode",
            "ProviderNum",
            "ReferralNet",
            "Assists",
            "Specialty",
            "State",
            "Suburb",
            "Surname",
            "FullName",
            "Title",
            "WorkPhone"
        };

        private static readonly string[] ConsultFields =
        {
            "pt_id_fk",
            "ConsultDate",
            "History",
            "DoctorId",
            "ConsultTime",
            "Diagnosis",
            "Plan",
            "Id",
            "Examination"
        };

        private static readonly string[] AllergyFields =
        {
            "Allergy",
            "Detail",
            "Id",
            "pt_id_fk"
        };

        private static readonly string[] MeasurementFields =
        {
            "pt_id_fk",
            "MeasurementDate",
            "Height",
            "Weight",
            "HeadCircumference",
            "Waist",
            "Bmi",
            "Hip",
            "HeartRate",
            "Id",
            "Diastolic",
            "Systolic",
        };

        private static readonly string[] DownloadedResultFields =
        {
            "Id",
            "pt_id_fk",
            "Addressee",
            "DocumentName",
            "Dob",
            "ReportDate",
            "ImportDate",
            "CollectionDate",
            "ReceivedDate",
            "FirstName",
            "Surname",
            "Result",
            "Test",
            "LabRef",
            "NormalOrAbnormal",
        };

        private static readonly string[] ChecklistFields =
        {
            "Id",
            "DateCreated",
            "Name",
            "PRCDRE_Id_Fk",
            "Provider",
            "PT_Id_Fk"
        };

        private static readonly string[] AccountHolderFields =
        {
            "AddressLine1",
            "AddressLine2",
            "DOB",
            "FirstName",
            "HomePhone",
            "FullName",
            "Fax",
            "Mobile",
            "Id",
            "Individual",
            "MedicareExpiry",
            "MedicareNum",
            "MedicareRefNum",
            "Organisation",
            "Postcode",
            "State",
            "Suburb",
            "Surname",
            "Title"
        };

        private static readonly string[] OpReportFields =
        {
            "Id",
            "ProviderKey",
            "PT_Id_Fk",
            "Assistant_AB_Id_Fk",
            "Anaesthetist_AB_Id_Fk",
            "ProcedureName",
            "Side",
            "Provider",
            "Hospital",
            "ProcedureDate",
            "StartTime",
            "EndTime",
            "AdmissionDate",
            "AdmissionTime",
            "FastingFrom",
            "DischargeDate",
            "DaysHospitalised",
            "ClinicalIndication",
            "Category",
            "Magnitude",
            "InfectionRisk",
            "ProcedureType",
            "Anaesthetic",
            "Prosthesis",
            "Finding",
            "Technique",
            "PostOp",
            "AdmissionOutcome",
            "FollowupDate",
            "FollowupOutcome",
            "AuditSummary",
            "PreopDiagnosis",
            "PostopDiagnosis",
        };

        private static readonly string[] DrugFields =
        {
            "UniqueCode",
            "Form",
            "Drug",
            "Quantity",
            "Strength",
            "Code",
            "ProdCode",
            "FormCode",
            "PackCode"
        };

        private static readonly string[] GraphicFields =
        {
            "Graphic.Description",
            "Graphic.Id",
            "Graphic.ImageDate",
            "Graphic.ModificationDate",
            "Graphic.PathName",
            "Graphic.pt_id_fk",
            "Graphic.RealName",
            "Patient.FirstName",
            "Patient.Surname"
        };

        private static readonly string[] ScriptFields =
        {
            "Preference.Id as DoctorId",
            "ScriptArchive.ApprovalNum",
            "ScriptArchive.AuthorityNum",
            "ScriptArchive.CreationDate",
            "ScriptArchive.Dose",
            "ScriptArchive.DrugIndexCode",
            "ScriptArchive.Id",
            "ScriptArchive.Medication",
            "ScriptArchive.Note",
            "ScriptArchive.PT_Id_fk",
            "ScriptArchive.Qty",
            "ScriptArchive.ScriptNum",
            "ScriptArchive.[Repeat] as [Repeat]"
        };

        private static readonly string[] WorkCoverClaimFields =
        {
            "WorkCoverClaim.Id",
            "WorkCoverClaim.PT_Id_Fk",
            "WorkCoverClaim.ClaimNum",
            "WorkCoverClaim.InjuryDate",
            "WorkCoverClaim.Injury",
            "WorkCoverClaim.EMPL_Id_Fk",
            "WorkCoverClaim.InjuryTime",
            "WorkCoverClaim.InjuryMechanism",
            "WorkCoverClaim.Location",
            "WorkCoverClaim.CaseManager",
            "AddressBook.FullName",
            "AddressBook.WorkPhone",
            "AddressBook.Mobile",
        };

        private static readonly string[] EmployerFields =
        {
            "Employer.Id",
            "Employer.Name",
            "Employer.AddressLine1",
            "Employer.AddressLine2",
            "Employer.Suburb",
            "Employer.State",
            "Employer.Postcode",
            "Employer.Phone1",
            "Employer.Phone2",
            "Employer.Fax",
            "Employer.Email",
            "Employer.Country",
            "Employer.Insurer_AH_Id_Fk",
        };

        private static readonly string[] DocumentFields =
        {
            "Title",
            "Note",
            "PT_Id_Fk",
            "DocumentDate",
            "DateModified",
            "Id",
            "[Primary]",
        };

        public IGenieSettingsService GenieSettingsService { get; set; }

        public bool TestConnection()
        {
            using (var connection = new OdbcConnection(GenieSettingsService.ConnectionString))
            {
                try
                {
                    connection.ConnectionTimeout = 1;
                    connection.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("Connection test failed.", ex);
                    return false;
                }
            }
        }

        public IEnumerable<GenieUser> GetUsers()
        {
            return QuerySql(UserFields, "Preference", null, GenieDataReaderReadExtensions.GetGenieUser)
                .Concat(GetMd3Users());
        }

        public IEnumerable<GenieRecall> GetRecalls()
        {
            return QuerySql(AllFields, "Recall", null, GenieDataReaderReadExtensions.GetGenieRecall);
        }

        public IEnumerable<GenieCurrentProblem> GetCurrentProblems()
        {
            return QuerySql(CurrentProblemFields, "CurrentProblem", null, GenieDataReaderReadExtensions.GetGenieCurrentProblem);
        }

        public IEnumerable<GenieConsultProblem> GetConsultProblems()
        {
            return QuerySql(ConsultationProblemFields, "ConsultationProblem", null, GenieDataReaderReadExtensions.GetGenieConsultProblem);
        }

        public IEnumerable<GeniePastHistory> GetPastHistory()
        {
            return QuerySql(PastHistoryFields, "PastHistory", null, GenieDataReaderReadExtensions.GetGeniePastHistory);
        }

        public IEnumerable<GenieDocument> GetDocuments()
        {
            return QuerySql(DocumentFields, "Document", "PT_Id_Fk <> 0", GenieDataReaderReadExtensions.GetGenieDocument);
        }

        public IEnumerable<GeniePatient> GetPatients(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("(CreationDate is null) or (CreationDate >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}')", start.Value)
                : null;
            return QuerySql(PatientFields, "Patient", where, GenieDataReaderReadExtensions.GetGeniePatient);
        }

        public IEnumerable<GenieAppointment> GetAppointments(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("(Cancelled <> true) and (PT_Id_Fk <> 0) and ((CreationDate >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}'))", start.Value)
                : "(Cancelled <> true) and (PT_Id_Fk <> 0)";
            return QuerySql(AppointmentFields, "Appt", where, GenieDataReaderReadExtensions.GetGenieAppointment);
        }

        public IEnumerable<GenieReferral> GetReferrals()
        {
            return QuerySql(AllFields, "Referral", null, GenieDataReaderReadExtensions.GetGenieReferral);
        }

        public IEnumerable<GenieAppointmentType> GetAppointmentTypes()
        {
            return QuerySql(AllFields, "ApptType", null, GenieDataReaderReadExtensions.GetGenieAppointmentTypes);
        }

        public IEnumerable<GenieContact> GetContacts()
        {
            return QuerySql(ContactFields, "AddressBook", null, GenieDataReaderReadExtensions.GetGenieContact);
        }

        public IEnumerable<GenieConsult> GetConsults(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((DateCreated >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}'))", start.Value)
                : null;
            return QuerySql(ConsultFields, "Consult", where, GenieDataReaderReadExtensions.GetGenieConsult);
        }

        public IEnumerable<GenieAllergy> GetAllergies()
        {
            var where = "Allergy is not null and Allergy <> ''";
            return QuerySql(AllergyFields, "Allergy", where, GenieDataReaderReadExtensions.GetGenieAllergy);
        }

        public IEnumerable<GenieMeasurement> GetMeasurements(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("(MeasurementDate >= '{0:yyyy-MM-dd}')", start.Value)
                : null;
            return QuerySql(MeasurementFields, "Measurement", where, GenieDataReaderReadExtensions.GetGenieMeasurement);
        }

        public IEnumerable<GenieIncomingLetter> GetIncomingLetters(DateTime? start)
        {
            return QueryXml<GenieIncomingLetter>("incomingletter.xml", "incomingletter", GenieXmlReadExtensions.GetGenieIncomingLetter);
        }

        public IEnumerable<GenieOutgoingLetter> GetOutgoingLetters(DateTime? start)
        {
            return QueryXml<GenieOutgoingLetter>("outgoingletter.xml", "outgoingletter", GenieXmlReadExtensions.GetGenieOutgoingLetter);
        }

        public IEnumerable<GenieTask> GetTasks(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((DateCreated >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}')) and TSK_Id_Fk <> 0 ", start.Value)
                : "TSK_Id_Fk <> 0";
            return QuerySql(AllFields, "Task", where, GenieDataReaderReadExtensions.GetGenieTask);
        }

        public IEnumerable<GenieVaccination> GetVaccinations(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((GivenDate >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}'))", start.Value)
                : null;
            return QuerySql(AllFields, "Vaccination", where, GenieDataReaderReadExtensions.GetGenieVaccination);
        }

        public IEnumerable<GenieDownloadedResult> GetDownloadedResults(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((ImportDate >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}'))", start.Value)
                : null;
            return QuerySql(DownloadedResultFields, "DownloadedResult", where, GenieDataReaderReadExtensions.GetGenieDownloadedResult)
                .Concat(GetMd3LaboratoryResults()
                    .Join(
                        GetPatients(start),
                        x => x.ExternalPatientId,
                        x => x.ExternalId.Split('_').Last(),
                        (i, o) =>
                        {
                            i.PatientId = o.Id;
                            i.FirstName = o.FirstName;
                            i.Surname = o.Surname;
                            return i;
                        }));
        }

        private IEnumerable<GenieDownloadedResult> GetMd3LaboratoryResults()
        {
            var xmlFilePaths = GetXmlFilePaths();
            return xmlFilePaths.Any()
                ? xmlFilePaths
                    .Select(o => QueryXml(
                        o,
                        GenieXmlReadExtensions.Md3LaboratoryResultTagName,
                        GenieXmlReadExtensions.GetGenieLaboratoryResultFromMd3)
                        .ToArray())
                    .SelectMany(x => x)
                    .GroupBy(x => x.Id)
                    .Select(x => x.FirstOrDefault())
                : Enumerable.Empty<GenieDownloadedResult>();
        }

        public IEnumerable<GenieGraphic> GetGraphics(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((Graphic.ImageDate >= '{0:yyyy-MM-dd}') or (Graphic.ModificationDate >= '{0:yyyy-MM-dd}'))", start.Value)
                : null;
            return QuerySql(GraphicFields, "Graphic inner join Patient on Graphic.pt_id_fk = Patient.Id", where, GenieDataReaderReadExtensions.GetGenieGraphic);
        }

        public IEnumerable<GenieChecklist> GetPatientChecklists()
        {
            return QuerySql(ChecklistFields, "CheckListNew", "PT_Id_Fk <> 0", GenieDataReaderReadExtensions.GetGenieChecklist);
        }

        public IEnumerable<GenieChecklist> GetChecklistTemplates()
        {
            return QuerySql(ChecklistFields, "CheckListNew", "PT_Id_Fk = 0", GenieDataReaderReadExtensions.GetGenieChecklist);
        }

        public IEnumerable<GenieChecklistField> GetChecklistFields()
        {
            return QuerySql(AllFields, "CheckListField", null, GenieDataReaderReadExtensions.GetGenieChecklistField);
        }

        public IEnumerable<GenieAccountHolder> GetAccountHolders()
        {
            return QuerySql(AccountHolderFields, "AccountHolder", null, GenieDataReaderReadExtensions.GetGenieAccountHolder);
        }

        public IEnumerable<GenieOpReport> GetOpReports(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("((ProcedureDate >= '{0:yyyy-MM-dd}') or (LastUpdated >= '{0:yyyyMMddHHmm}'))", start.Value)
                : null;
            return QuerySql(OpReportFields, "Procedures", where, GenieDataReaderReadExtensions.GetGenieOpReport);
        }

        public IEnumerable<GenieQuoteItem> GetQuoteItems()
        {
            return QuerySql(AllFields, "Operation", null, GenieDataReaderReadExtensions.GetGenieQuoteItem);
        }

        public IEnumerable<GenieScript> GetScripts(DateTime? start)
        {
            return QuerySql(ScriptFields, "ScriptArchive inner join Preference on ScriptArchive.Creator = Preference.Username", null, GenieDataReaderReadExtensions.GetGenieScript)
                .Concat(GetMd3Scripts()
                    .Join(
                        GetPatients(start),
                        x => x.ExternalPatientId,
                        x => x.ExternalId.Split('_').Last(),
                        (i, o) =>
                        {
                            i.PatientId = o.Id;
                            return i;
                        }));
        }

        public IEnumerable<GenieDrug> GetDrugs()
        {
            return QuerySql(DrugFields, "DrugIndex", null, GenieDataReaderReadExtensions.GetGenieDrug)
                .Concat(GetMd3Drugs());
        }

        public IEnumerable<GeniePregnancy> GetOnGRecords()
        {
            return QuerySql(AllFields, "Pregnancy", null, GenieDataReaderReadExtensions.GetGeniePregnancy);
        }

        public IEnumerable<GenieObstetricHistory> GetOnGHistoryRecords()
        {
            return QuerySql(AllFields, "ObstetricHistory", "Id not in (select OBSHX_Id_Fk from Pregnancy)", GenieDataReaderReadExtensions.GetGenieObstetricHistory);
        }

        public IEnumerable<GenieAntenatalVisit> GetAntenatalVisits()
        {
            return QuerySql(AllFields, "Antenatal", null, GenieDataReaderReadExtensions.GetGenieAntenatalVisit);
        }

        public IEnumerable<GenieInterestedParty> GetInterestedParties()
        {
            return QuerySql(AllFields, "InterestedParty", null, GenieDataReaderReadExtensions.GetGenieInterestedParties);
        }

        public IEnumerable<GenieComplication> GetComplications(DateTime? start)
        {
            var where = start.HasValue
                ? string.Format("ComplicationDate >= '{0:yyyy-MM-dd}'", start.Value)
                : null;
            return QuerySql(AllFields, "Complication", where, GenieDataReaderReadExtensions.GetGenieComplication);
        }

        public IEnumerable<GenieEmployer> GetEmployers()
        {
            return QuerySql(AllFields, "Employer", null, GenieDataReaderReadExtensions.GetGenieEmployer);
        }

        public IEnumerable<GenieAccountHolder> GetInsurers()
        {
            return QuerySql(AccountHolderFields, "AccountHolder", "Individual <> true", GenieDataReaderReadExtensions.GetGenieAccountHolder);
        }

        public IEnumerable<GenieWorkCoverClaim> GetWorkCoverClaims()
        {
            var employersWithWorkcoverClaims = QuerySql(EmployerFields, "Employer", "Id in (SELECT DISTINCT EMPL_Id_Fk From WorkCoverClaim WHERE EMPL_Id_Fk is not null)", GenieDataReaderReadExtensions.GetGenieEmployer);
            return QuerySql(WorkCoverClaimFields, "WorkCoverClaim c left join AddressBook ab on c.AB_Id_Fk = ab.Id", null, GenieDataReaderReadExtensions.GetGenieWorkCoverClaim)
                .Join(
                    employersWithWorkcoverClaims,
                    x => x.EmployerId,
                    x => x.Id,
                    (x, y) =>
                    {
                        x.InsurerId = y.InsurerId;
                        return x;
                    });
        }

        public IEnumerable<GenieUser> GetMd3Users()
        {
            var xmlFilePaths = GetXmlFilePaths();
            if (!xmlFilePaths.Any())
            {
                return Enumerable.Empty<GenieUser>();
            }

            var practitioners = xmlFilePaths.Select(o => QueryXml(o, GenieXmlReadExtensions.Md3PractitionerTagName, GenieXmlReadExtensions.GetGenieUserFromMd3Practitioner).ToArray());
            var resources = xmlFilePaths.Select(o => QueryXml(o, GenieXmlReadExtensions.Md3ResourceTagName, GenieXmlReadExtensions.GetGenieUserFromMd3Resource).ToArray());

            return practitioners
                .Concat(resources)
                .SelectMany(x => x)
                .Where(x => x != null)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .GroupBy(x => x.UserName)
                .Select(x => x.FirstOrDefault());
        }

        public IEnumerable<GenieScript> GetMd3Scripts()
        {
            var xmlFilePaths = GetXmlFilePaths();
            return xmlFilePaths.Any()
                ? xmlFilePaths
                    .Select(o => QueryXml(o, GenieXmlReadExtensions.Md3ScriptTagName, GenieXmlReadExtensions.GetGenieScriptFromMd3Script).ToArray())
                    .SelectMany(x => x)
                : Enumerable.Empty<GenieScript>();
        }

        public IEnumerable<GenieDrug> GetMd3Drugs()
        {
            var xmlFilePaths = GetXmlFilePaths();
            return xmlFilePaths.Any()
                ? xmlFilePaths
                    .Select(o => QueryXml(o, GenieXmlReadExtensions.Md3ScriptTagName, GenieXmlReadExtensions.GetGenieDrugFromMd3Drug).ToArray())
                    .SelectMany(x => x)
                    .GroupBy(x => x.Id).Select(d => d.First())
                : Enumerable.Empty<GenieDrug>();
        }

        private IEnumerable<TEntity> QueryXml<TEntity>(string filename, XName name, Func<XElement, TEntity> create)
        {
            var filepath = GetXmlFilePath(filename);
            if (!File.Exists(filepath))
            {
                Log.ErrorFormat("Failed to open Genie XML exported file: {0}", filepath);
                yield break;
            }

            using (var reader = XmlReader.Create(filepath))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == name)
                        {
                            var el = XNode.ReadFrom(reader) as XElement;
                            if (el != null && !el.IsEmpty)
                            {
                                yield return create(el);
                            }
                        }
                    }
                }
            }
        }

        private string GetXmlFilePath(string filename)
        {
            return Path.Combine(GenieSettingsService.XmlExportPath, filename);
        }

        private string[] GetXmlFilePaths()
        {
            return Directory.Exists(GenieSettingsService.Md3XmlPath)
                ? Directory.GetFiles(GenieSettingsService.Md3XmlPath, "*.xml", SearchOption.AllDirectories)
                : new string[] { };
        }

        private IEnumerable<TEntity> QuerySql<TEntity>(string[] fields, string table, string where, Func<DbDataReader, Dictionary<string, int>, TEntity> create)
        {
            return DbHelper.QuerySql(GenieDbProvider, GenieSettingsService.ConnectionString, fields, table, where, create, Log);
        }
    }
}