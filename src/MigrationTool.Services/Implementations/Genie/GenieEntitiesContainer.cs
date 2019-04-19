using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers.Genie;
using MigrationTool.Services.Interfaces;
using MigrationTool.Services.Interfaces.Genie;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations.Genie
{
    internal class GenieEntitiesContainer : IGenieEntitiesContainer
    {
        private const string GenieFiledDataPrefix = ";0;";

        public IGenieRepository GenieRepository { get; set; }

        public IGenieSettingsService GenieSettingsService { get; set; }

        public ITextConverter TextConverter { get; set; }

        public MigrationSourceSystem SourceSystem
        {
            get { return MigrationSourceSystem.Genie; }
        }

        public MigrationEntity[] SupportedEntityTypes
        {
            get { return DataSources.Keys.ToArray(); }
        }

        public void ProcessEntities(Action<IEnumerable<IMigrationEntity>> processEntities, MigrationEntity entityType, MigrationArgs args)
        {
            var entities = DataSources[entityType](args);
            processEntities(entities);
        }

        private Dictionary<MigrationEntity, Func<MigrationArgs, IEnumerable<IMigrationEntity>>> localDataSources;

        private Dictionary<MigrationEntity, Func<MigrationArgs, IEnumerable<IMigrationEntity>>> DataSources
        {
            get
            {
                if (localDataSources == null)
                {
                    localDataSources = new Dictionary<MigrationEntity, Func<MigrationArgs, IEnumerable<IMigrationEntity>>>
                    {
                        { MigrationEntity.AccountHolders, GetDataAccountHolders },
                        { MigrationEntity.Allergies, GetDataAllergies },
                        { MigrationEntity.Appointments, GetDataAppointments },
                        { MigrationEntity.AppointmentTypes, GetDataAppointmentTypes },
                        { MigrationEntity.ChecklistTemplates, GetDataChecklistTemplates },
                        { MigrationEntity.Checklists, GetDataChecklists },
                        { MigrationEntity.Consults, GetDataConsults },
                        { MigrationEntity.Contacts, GetDataContacts },
                        { MigrationEntity.Documents, GetDataDocuments },
                        { MigrationEntity.LaboratoryResults, GetDataLaboratoryResults },
                        { MigrationEntity.Letters, GetDataOutgoingLetters },
                        { MigrationEntity.Measurements, GetDataMeasurements },
                        { MigrationEntity.OpReports, GetDataOpReports },
                        { MigrationEntity.Patients, GetDataPatients },
                        { MigrationEntity.Referrals, GetDataReferrals },
                        { MigrationEntity.Tasks, GetDataTasks },
                        { MigrationEntity.Users, GetDataUsers },
                        { MigrationEntity.Scripts, GetDataScripts },
                        { MigrationEntity.Vaccinations, GetDataVaccinations },
                        { MigrationEntity.OnGHistoryRecords, GetDataOnGHistoryRecords },
                        { MigrationEntity.OnGRecords, GetDataOnGRecords },
                        { MigrationEntity.OnGAntenatalVisits, GetDataOnGAntenatalConsults },
                        { MigrationEntity.Recalls, GetDataRecalls },
                        { MigrationEntity.IncomingLetters, GetDataIncomingLetters },
                        { MigrationEntity.InterestedParties, GetDataInterestedParties },
                        { MigrationEntity.Companies, GetDataCompanies },
                        { MigrationEntity.WorkCoverClaims, GetDataWorkcoverClaims },
                    };
                }
                return localDataSources;
            }
        }

        private IEnumerable<IMigrationEntity> GetDataAccountHolders(MigrationArgs args)
        {
            var accountHolders = GenieRepository.GetAccountHolders();
            var patients = GenieRepository
                .GetPatients(null)
                .GroupBy(o => o.AccountHolderId)
                .Where(o => o.Key != 0)
                .ToDictionary(o => o.Key, o => o.Select(y => y.Id).ToArray());

            return accountHolders
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationAccountHolder(o, patients));
        }

        private IEnumerable<IMigrationEntity> GetDataAllergies(MigrationArgs args)
        {
            var allergies = GenieRepository
                .GetAllergies()
                .GroupBy(o => o.PatientId, o => o.Allergy)
                .ToArray();

            return allergies
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationAllergy(o));
        }

        private IEnumerable<IMigrationEntity> GetDataAppointments(MigrationArgs args)
        {
            var appointments = GenieRepository.GetAppointments(args.Start);
            var users = GenieRepository
                .GetUsers()
                .ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);

            var appointmentTypes = GenieRepository
                .GetAppointmentTypes().GroupBy(o => o.Note).Select(o => o.First())
                .ToDictionary(o => o.Note, o => o.Id, StringComparer.InvariantCultureIgnoreCase);

            return appointments
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationAppointment(o, users, appointmentTypes));
        }

        private IEnumerable<IMigrationEntity> GetDataAppointmentTypes(MigrationArgs args)
        {
            return GenieRepository
                .GetAppointmentTypes()
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationAppointmentType(o));
        }

        private IEnumerable<IMigrationEntity> GetDataChecklistTemplates(MigrationArgs args)
        {
            var checklistTemplates = GenieRepository.GetChecklistTemplates();
            var checklistFields = GenieRepository
                .GetChecklistFields()
                .Where(x => x.Type != GenieMigrationHelper.GeniePicture)
                .Select(x =>
                {
                    if (x.Type == GenieMigrationHelper.GenieText && x.FieldData != GenieFiledDataPrefix &&
                        x.FieldData.StartsWith(GenieFiledDataPrefix))
                    {
                        x.FieldData = x.FieldData.Substring(GenieFiledDataPrefix.Length - 1);
                        x.Type = GenieMigrationHelper.GenieRadio;
                    }
                    return x;
                })
                .GroupBy(x => x.ChecklistId, x => x)
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Position).ToArray());

            return checklistTemplates
                .Select(x => (IMigrationEntity)GenieMigrationHelper.ToMigrationChecklistTemplate(x, checklistFields));
        }

        private IEnumerable<IMigrationEntity> GetDataChecklists(MigrationArgs args)
        {
            var checklists = GenieRepository.GetPatientChecklists();

            var templateChecklistFields = GenieRepository.GetChecklistFields();
            var templateChecklistFieldsDictionary = templateChecklistFields.ToDictionary(x => x.Id);

            var users = GenieRepository.GetUsers().ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);

            var checklistFields = templateChecklistFields
                .Where(x => x.Type != GenieMigrationHelper.GeniePicture)
                .GroupBy(x => x.ChecklistId, x => x)
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Position).ToArray());

            return checklists
                .Select(x => (IMigrationEntity)GenieMigrationHelper.ToMigrationChecklist(x, users, checklistFields, templateChecklistFieldsDictionary));
        }

        private IEnumerable<IMigrationEntity> GetDataConsults(MigrationArgs args)
        {
            var consultProblems = GenieRepository.GetConsultProblems()
                .GroupBy(x => x.ConsultId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());

            return GenieRepository.GetConsults(args.Start)
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationConsult(o, consultProblems));
        }

        private IEnumerable<IMigrationEntity> GetDataContacts(MigrationArgs args)
        {
            return GenieRepository.GetContacts()
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationContact(o));
        }

        private IEnumerable<IMigrationEntity> GetDataDocuments(MigrationArgs args)
        {
            var documents = GenieRepository.GetGraphics(args.Start);

            var zeroPath = Path.Combine(GenieSettingsService.DocumentsPath, "0");
            var zeroFiles = Directory.Exists(zeroPath)
                ? Directory.GetFiles(zeroPath, "*", SearchOption.AllDirectories)
                : new string[] { };

            return documents
                .Select(o =>
                    (IMigrationEntity)GenieMigrationHelper.ToMigrationDocument(zeroFiles, o, GenieSettingsService.DocumentsPath));
        }

        private IEnumerable<IMigrationEntity> GetDataLaboratoryResults(MigrationArgs args)
        {
            var laboratoryResults = GenieRepository.GetDownloadedResults(args.Start);
            var users = GenieRepository.GetUsers()
                .ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);

            return laboratoryResults
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationLaboratoryResult(o, users, GenieSettingsService.LaboratoryResultsPath, TextConverter.TextToHtml));
        }

        private IEnumerable<IMigrationEntity> GetDataOutgoingLetters(MigrationArgs args)
        {
            var letters = GenieRepository.GetOutgoingLetters(args.Start);
            var users = GenieRepository.GetUsers().ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);
            return letters
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationLetter(o, users, TextConverter.RtfToHtml));
        }

        private IEnumerable<IMigrationEntity> GetDataIncomingLetters(MigrationArgs args)
        {
            var letters = GenieRepository.GetIncomingLetters(args.Start);
            var users = GenieRepository.GetUsers();
            var contacts = GenieRepository.GetContacts();

            return letters
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationDocument(o, users, contacts, TextConverter.RtfToHtml));
        }

        private IEnumerable<IMigrationEntity> GetDataMeasurements(MigrationArgs args)
        {
            return GenieRepository.GetMeasurements(args.Start)
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationMeasurement(o));
        }

        private IEnumerable<IMigrationEntity> GetDataOpReports(MigrationArgs args)
        {
            var reports = GenieRepository.GetOpReports(args.Start);
            var users = GenieRepository.GetUsers()
                .ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);

            var complications = GenieRepository.GetComplications(args.Start)
                .GroupBy(x => x.ProcedureId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var checklists = GenieRepository.GetPatientChecklists()
                .GroupBy(x => x.PRCDRE_Id_Fk, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var quoteItems = GenieRepository.GetQuoteItems()
                .GroupBy(x => x.OpReportId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var templateChecklistFields = GenieRepository.GetChecklistFields();
            var templateChecklistFieldsDictionary = templateChecklistFields.ToDictionary(x => x.Id);

            var checklistFields = templateChecklistFields
                .Where(x => x.Type != GenieMigrationHelper.GeniePicture)
                .GroupBy(x => x.ChecklistId, x => x)
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Position).ToArray());

            return reports
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationOpReport(o, users, complications, checklists, checklistFields, templateChecklistFieldsDictionary, quoteItems));
        }

        private IEnumerable<IMigrationEntity> GetDataPatients(MigrationArgs args)
        {
            var accountHolders = GenieRepository.GetAccountHolders().ToDictionary(o => o.Id, o => o);
            var currentProblems = GenieRepository.GetCurrentProblems()
                .GroupBy(x => x.PatientId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());
            var pastHistory = GenieRepository.GetPastHistory()
                .GroupBy(x => x.PatientId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());
            var notes = GenieRepository.GetDocuments()
                .GroupBy(x => x.PatientId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var patients = GenieRepository.GetPatients(args.Start);
            return patients
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationPatient(o, accountHolders, currentProblems, pastHistory, notes));
        }

        private IEnumerable<IMigrationEntity> GetDataReferrals(MigrationArgs args)
        {
            var users = GenieRepository.GetUsers()
                .ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);
            return GenieRepository.GetReferrals()
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationReferral(o, users));
        }

        private IEnumerable<IMigrationEntity> GetDataTasks(MigrationArgs args)
        {
            var tasks = GenieRepository.GetTasks(args.Start);
            var users = GenieRepository.GetUsers().ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);
            return tasks.Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationTask(o, users));
        }

        private IEnumerable<IMigrationEntity> GetDataUsers(MigrationArgs args)
        {
            return GenieRepository.GetUsers()
                .Select(o => (IMigrationEntity)o.ToMigrationUser());
        }

        private IEnumerable<IMigrationEntity> GetDataOnGRecords(MigrationArgs args)
        {
            var users = GenieRepository.GetUsers().ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);
            return GenieRepository.GetOnGRecords()
                .Select(o => (IMigrationEntity)o.ToMigrationOnGRecord(users));
        }

        private IEnumerable<IMigrationEntity> GetDataOnGAntenatalConsults(MigrationArgs args)
        {
            var users = GenieRepository.GetUsers().ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);
            return GenieRepository.GetAntenatalVisits()
                .Select(o => (IMigrationEntity)o.ToMigrationAntenatalVisit(users));
        }

        private IEnumerable<IMigrationEntity> GetDataOnGHistoryRecords(MigrationArgs args)
        {
            return GenieRepository.GetOnGHistoryRecords()
                .Select(o => (IMigrationEntity)o.ToMigrationOnGHistoryRecord());
        }

        private IEnumerable<IMigrationEntity> GetDataRecalls(MigrationArgs args)
        {
            return GenieRepository.GetRecalls()
                .Select(o => (IMigrationEntity)o.ToMigrationRecall());
        }

        private IEnumerable<IMigrationEntity> GetDataVaccinations(MigrationArgs args)
        {
            return GenieRepository.GetVaccinations(args.Start)
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationVaccination(o));
        }

        private IEnumerable<IMigrationEntity> GetDataScripts(MigrationArgs args)
        {
            var drugs = GenieRepository.GetDrugs().ToDictionary(x => x.Id);

            return GenieRepository.GetScripts(args.Start)
                .Select(x => (IMigrationEntity)GenieMigrationHelper.ToMigrationScript(x, drugs));
        }

        private IEnumerable<IMigrationEntity> GetDataInterestedParties(MigrationArgs args)
        {
            return GenieRepository.GetInterestedParties()
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationInterestedParty(o));
        }

        private IEnumerable<IMigrationEntity> GetDataCompanies(MigrationArgs args)
        {
            return GenieRepository.GetEmployers()
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationCompany(o))
                .Concat(GenieRepository.GetInsurers()
                    .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationCompany(o)));
        }

        private IEnumerable<IMigrationEntity> GetDataWorkcoverClaims(MigrationArgs args)
        {
            return GenieRepository.GetWorkCoverClaims()
                .Select(o => (IMigrationEntity)GenieMigrationHelper.ToMigrationWorkCoverClaim(o));
        }
    }
}