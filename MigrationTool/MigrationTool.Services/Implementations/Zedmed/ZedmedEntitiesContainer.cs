using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Entities.Zedmed;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Helpers.Zedmed;
using MigrationTool.Services.Interfaces.Zedmed;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations.Zedmed
{
    internal class ZedmedEntitiesContainer : IZedmedEntitiesContainer
    {
        public IZedmedRepository ZedmedRepository { get; set; }

        public MigrationEntity[] SupportedEntityTypes
        {
            get { return DataSources.Keys.ToArray(); }
        }

        public MigrationSourceSystem SourceSystem
        {
            get { return MigrationSourceSystem.Zedmed; }
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
                        { MigrationEntity.Letters, GetDataLetters },
                        { MigrationEntity.Measurements, GetDataMeasurements },
                        { MigrationEntity.Patients, GetDataPatients },
                        { MigrationEntity.Referrals, GetDataReferrals },
                        { MigrationEntity.Tasks, GetDataTasks },
                        { MigrationEntity.Users, GetDataUsers },
                        { MigrationEntity.Scripts, GetDataScripts },
                        { MigrationEntity.Vaccinations, GetDataVaccinations },
                        { MigrationEntity.InterestedParties, GetDataInterestedParties },
                        { MigrationEntity.Companies, GetDataCompanies },
                        { MigrationEntity.Recalls, GetDataRecalls },
                        { MigrationEntity.WorkCoverClaims, GetDataWorkcoverClaims },
                    };
                }
                return localDataSources;
            }
        }

        private IEnumerable<IMigrationEntity> GetDataWorkcoverClaims(MigrationArgs arg)
        {
            return ZedmedRepository.GetWorkCoverClaims()
                .Select(x => (IMigrationEntity)x.ToMigrationWorkCoverClaim());
        }

        private IEnumerable<IMigrationEntity> GetDataDocuments(MigrationArgs args)
        {
            return ZedmedRepository.GetAttachments(args.Start)
                .Select(o => (IMigrationEntity)o.ToMigrationDocument())
                .Concat(ZedmedRepository.GetImageDocuments()
                    .Select(o => (IMigrationEntity)o.ToMigrationDocument()));
        }

        private IEnumerable<IMigrationEntity> GetDataChecklists(MigrationArgs args)
        {
            var templateControls = ZedmedRepository.GetTemplateControls()
                .GroupBy(x => x.TemplateId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var templates = ZedmedRepository.GetChecklistTemplates().ToArray();

            var encounterNotes = ZedmedRepository.GetEncounterNotes()
                .GroupJoin(
                    ZedmedRepository.GetEncounters(args.Start).ToDictionary(x => x.Id),
                    x => x.EncounterId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.Encounter = o.Select(x => x.Value).FirstOrDefault() ?? new ZedmedEncounter();
                        return i;
                    }
                );

            var checklistHtmlTagRegex = new Regex(string.Format(
                ZedmedMigrationHelper.ChecklistHtmlDivTagPattern,
                templates.Select(x => Regex.Escape(x.Name.ToUpper())).AsStringAlternation()));

            var checklists = encounterNotes
                .Select(x => GetChecklistsFromEncounterNote(
                    x,
                    templates,
                    checklistHtmlTagRegex))
                .SelectMany(x => x.ToArray());

            return checklists
                .Select(o => (IMigrationEntity)o.ToMigrationChecklist(templateControls));
        }

        private static IEnumerable<ZedmedChecklist> GetChecklistsFromEncounterNote(ZedmedEncounterNote encounterNote, ZedmedChecklistTemplate[] templates, Regex checklistHtmlTagRegex)
        {
            return checklistHtmlTagRegex
                .Matches(WebUtility.HtmlDecode(encounterNote.SectionNotes))
                .Cast<Match>()
                .Select(m => new
                {
                    Name = m.Groups[2].Value,
                    Value = m.Groups[4].Value,
                })
                .Select(x =>
                {
                    var template = templates.FirstOrDefault(t => t.Name.EqualsIgnoreCase(x.Name)) ?? new ZedmedChecklistTemplate();
                    return new ZedmedChecklist
                    {
                        Id = encounterNote.Id + "-" + template.Id,
                        TemplateId = template.Id,
                        Name = template.Name,
                        PatientId = encounterNote.Encounter.PatientId,
                        SectionNotes = x.Value,
                        DateCreated = encounterNote.Encounter.StartDateTime,
                        DoctorId = encounterNote.Encounter.DoctorCode,
                    };
                });
        }

        private IEnumerable<IMigrationEntity> GetDataChecklistTemplates(MigrationArgs args)
        {
            var templateControls = ZedmedRepository.GetTemplateControls()
                .GroupBy(x => x.TemplateId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());

            return ZedmedRepository.GetChecklistTemplates()
                .Select(o => (IMigrationEntity)o.ToMigrationChecklistTemplate(templateControls));
        }

        private IEnumerable<IMigrationEntity> GetDataInterestedParties(MigrationArgs args)
        {
            return ZedmedRepository.GetInterestedParties()
                .Select(o => (IMigrationEntity)o.ToMigrationInterestedParty());
        }

        private IEnumerable<IMigrationEntity> GetDataMeasurements(MigrationArgs args)
        {
            return ZedmedRepository.GetMeasurements(args.Start)
                .GroupBy(x => x.Id)
                .Select(o => (IMigrationEntity)o.ToMigrationMeasurement());
        }

        private IEnumerable<IMigrationEntity> GetDataTasks(MigrationArgs args)
        {
            var tasks = ZedmedRepository.GetTasks(args.Start);
            var users = ZedmedRepository.GetUsers()
                .Where(o => !string.IsNullOrWhiteSpace(o.UserName))
                .ToDictionary(o => o.UserName, o => o.Id, StringComparer.InvariantCultureIgnoreCase);
            return tasks.Select(o => (IMigrationEntity)o.ToMigrationTask(users));
        }

        private IEnumerable<IMigrationEntity> GetDataVaccinations(MigrationArgs args)
        {
            return ZedmedRepository.GetVaccinations(args.Start)
                .Select(o => (IMigrationEntity)o.ToMigrationVaccination());
        }

        private IEnumerable<IMigrationEntity> GetDataUsers(MigrationArgs args)
        {
            return ZedmedRepository.GetUsers()
                .Select(o => (IMigrationEntity)o.ToMigrationUser());
        }

        private IEnumerable<IMigrationEntity> GetDataReferrals(MigrationArgs args)
        {
            return ZedmedRepository.GetReferrals()
                .Select(o => (IMigrationEntity)o.ToMigrationReferral());
        }

        private IEnumerable<IMigrationEntity> GetDataPatients(MigrationArgs args)
        {
            var accountHolders = ZedmedRepository.GetAccountHolders().ToDictionary(o => o.Id);
            var patientProblems = ZedmedRepository.GetPatientProblems()
                .GroupBy(x => x.PatientId, x => x)
                .ToDictionary(x => x.Key, x => x.ToArray());

            return ZedmedRepository.GetPatients()
                .Select(o => (IMigrationEntity)o.ToMigrationPatient(accountHolders, patientProblems));
        }

        private IEnumerable<IMigrationEntity> GetDataContacts(MigrationArgs args)
        {
            return ZedmedRepository.GetContacts()
                .Select(o => (IMigrationEntity)o.ToMigrationContact());
        }

        private IEnumerable<IMigrationEntity> GetDataAccountHolders(MigrationArgs args)
        {
            var patients = ZedmedRepository.GetPatients()
                .Where(o => o.AccountPayerType.EqualsIgnoreCase("P"))
                .GroupBy(o => o.AccounPayerId)
                .ToDictionary(o => o.Key, o => o.Select(x => x.Id).ToArray());

            return ZedmedRepository.GetAccountHolders()
                .Select(o => (IMigrationEntity)o.ToMigrationAccountHolder(patients));
        }

        private IEnumerable<IMigrationEntity> GetDataCompanies(MigrationArgs args)
        {
            return ZedmedRepository.GetAccountPayers()
                .Select(o => (IMigrationEntity)o.ToMigrationCompany());
        }

        private IEnumerable<IMigrationEntity> GetDataAllergies(MigrationArgs args)
        {
            var allergies = ZedmedRepository.GetAllergies()
                .GroupBy(o => o.PatientId, o => o.AllergyDescription);

            return allergies
                .Select(o => (IMigrationEntity)o.ToMigrationAllergy());
        }

        private IEnumerable<IMigrationEntity> GetDataConsults(MigrationArgs args)
        {
            var checklistHtmlTagRegex = new Regex(string.Format(
                ZedmedMigrationHelper.ChecklistHtmlDivTagPattern,
                ZedmedRepository.GetChecklistTemplates().Select(x => Regex.Escape(x.Name)).AsStringAlternation()));

            var encounterNotes = ZedmedRepository.GetEncounterNotes()
                .GroupBy(x => x.EncounterId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            return ZedmedRepository.GetEncounters(args.Start)
                .Select(o => (IMigrationEntity)o.ToMigrationConsult(encounterNotes, checklistHtmlTagRegex));
        }

        private IEnumerable<IMigrationEntity> GetDataLetters(MigrationArgs arg)
        {
            return ZedmedRepository.GetLetters()
                .Select(o => (IMigrationEntity)o.ToMigrationLetter());
        }

        private IEnumerable<IMigrationEntity> GetDataScripts(MigrationArgs arg)
        {
            var drugs = ZedmedRepository.GetDrugs()
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .ToDictionary(x => x.Id);
            var scripts = ZedmedRepository.GetScripts()
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault());

            return scripts
                .Select(o => (IMigrationEntity)o.ToMigrationScript(drugs));
        }

        private IEnumerable<IMigrationEntity> GetDataAppointmentTypes(MigrationArgs arg)
        {
            return ZedmedRepository.GetAppointmentTypes()
                .Select(o => (IMigrationEntity)o.ToMigrationAppointmentType());
        }

        private IEnumerable<IMigrationEntity> GetDataAppointments(MigrationArgs arg)
        {
            return ZedmedRepository.GetAppointments(arg.Start)
                .Select(o => (IMigrationEntity)o.ToMigrationAppointment());
        }

        private IEnumerable<IMigrationEntity> GetDataLaboratoryResults(MigrationArgs args)
        {
            var patients = ZedmedRepository.GetPatients().ToDictionary(o => o.Id);

            return ZedmedRepository.GetLaboratoryResults()
                .Select(o => (IMigrationEntity)o.ToMigrationLaboratoryResult(patients));
        }

        private IEnumerable<IMigrationEntity> GetDataRecalls(MigrationArgs args)
        {
            return ZedmedRepository.GetRecalls()
                .Select(o => (IMigrationEntity)o.ToMigrationRecall());
        }
    }
}