using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers.Shexie;
using MigrationTool.Services.Interfaces.Shexie;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations.Shexie
{
    internal class ShexieEntitiesContainer : IShexieEntitiesContainer
    {
        private const string LetterPath = @"\Letters\";

        private const string AllergiesName = "Allergies";

        private const string HealthConditionName = "Health Condition";

        public IShexieRepository ShexieRepository { get; set; }

        public IShexieSettingsService ShexieSettingsService { get; set; }

        private Dictionary<MigrationEntity, Func<MigrationArgs, IEnumerable<IMigrationEntity>>> localDataSources;

        private Dictionary<MigrationEntity, Func<MigrationArgs, IEnumerable<IMigrationEntity>>> DataSources
        {
            get
            {
                if (localDataSources == null)
                {
                    localDataSources = new Dictionary<MigrationEntity, Func<MigrationArgs, IEnumerable<IMigrationEntity>>>
                    {
                        { MigrationEntity.Users, GetDataUsers },
                        { MigrationEntity.Contacts, GetDataContacts },
                        { MigrationEntity.Companies, GetDataCompanies },
                        { MigrationEntity.Patients, GetDataPatients },
                        { MigrationEntity.OpReports, GetDataOpReports },
                        { MigrationEntity.Referrals, GetDataReferrals },
                        { MigrationEntity.Tasks, GetDataTasks },
                        { MigrationEntity.AppointmentTypes, GetDataAppointmentTypes },
                        { MigrationEntity.Appointments, GetDataAppointments },
                        { MigrationEntity.Consults, GetDataConsults },
                        { MigrationEntity.Scripts, GetDataScripts },
                        { MigrationEntity.LaboratoryResults, GetDataLaboratoryResults },
                        { MigrationEntity.Documents, GetDataDocuments },
                        { MigrationEntity.Letters, GetDataLetters },
                        { MigrationEntity.Recalls, GetDataRecalls },
                        { MigrationEntity.Allergies, GetDataAllergies },
                        { MigrationEntity.InterestedParties, GetDataInterestedParties },
                    };
                }
                return localDataSources;
            }
        }

        public MigrationSourceSystem SourceSystem
        {
            get { return MigrationSourceSystem.Shexie; }
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

        private IEnumerable<IMigrationEntity> GetDataUsers(MigrationArgs args)
        {
            return ShexieRepository.GetUsers().Select(o => (IMigrationEntity)o.ToMigrationUser())
                .Union(
                    ShexieRepository.GetProviders().Select(o => (IMigrationEntity)o.ToMigrationUser()));
        }

        private IEnumerable<IMigrationEntity> GetDataContacts(MigrationArgs args)
        {
            return ShexieRepository.GetContacts()
                .Select(o => (IMigrationEntity)o.ToMigrationContact());
        }

        private IEnumerable<IMigrationEntity> GetDataCompanies(MigrationArgs args)
        {
            return ShexieRepository.GetCompanies()
                .Select(o => (IMigrationEntity)o.ToMigrationCompany());
        }

        private IEnumerable<IMigrationEntity> GetDataPatients(MigrationArgs args)
        {
            var notes = ShexieRepository.GetPatientHistories()
                .GroupBy(x => x.PatientId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var statistics = ShexieRepository.GetStatistics()
                .GroupBy(x => x.PatientId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var healthConditionAnalyses = ShexieRepository.GetAnalyses()
                .Where(x => string.Equals(x.Desc, HealthConditionName, StringComparison.InvariantCultureIgnoreCase))
                .GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            return ShexieRepository.GetPatients()
                .Select(o => (IMigrationEntity)o.ToMigrationPatient(notes, statistics, healthConditionAnalyses));
        }

        private IEnumerable<IMigrationEntity> GetDataOpReports(MigrationArgs args)
        {
            return ShexieRepository.GetOpReports()
                .Select(o => (IMigrationEntity)o.ToMigrationOpReport());
        }

        private IEnumerable<IMigrationEntity> GetDataReferrals(MigrationArgs args)
        {
            return ShexieRepository.GetReferrals()
                .Select(o => (IMigrationEntity)o.ToMigrationReferral());
        }

        private IEnumerable<IMigrationEntity> GetDataTasks(MigrationArgs args)
        {
            return ShexieRepository.GetAlarms()
                .Select(o => (IMigrationEntity)o.ToMigrationTask());
        }

        private IEnumerable<IMigrationEntity> GetDataAppointmentTypes(MigrationArgs args)
        {
            return ShexieRepository.GetAppointmentTypes()
                .Select(o => (IMigrationEntity)o.ToMigrationAppointmentType());
        }

        private IEnumerable<IMigrationEntity> GetDataAppointments(MigrationArgs args)
        {
            return ShexieRepository.GetAppointments()
                .Select(o => (IMigrationEntity)o.ToMigrationAppointment());
        }

        private IEnumerable<IMigrationEntity> GetDataScripts(MigrationArgs args)
        {
            return ShexieRepository.GetScripts()
                .Select(o => (IMigrationEntity)o.ToMigrationScript());
        }

        private IEnumerable<IMigrationEntity> GetDataLaboratoryResults(MigrationArgs args)
        {
            var resInfoArray = ShexieRepository.GetLaboratoryResults().ToArray();
            var result = resInfoArray
                .Select(o => (IMigrationEntity)o.ToMigrationLaboratoryResult(ShexieSettingsService.DocumentsPath));
            return result;
        }

        private IEnumerable<IMigrationEntity> GetDataDocuments(MigrationArgs args)
        {
            return ShexieRepository.GetAttachments()
                .Where(x => !x.Path.Contains(LetterPath) || !IsWordFile(x.Path))
                .Select(o => (IMigrationEntity)o.ToMigrationDocument(ShexieSettingsService.DocumentsPath));
        }

        private IEnumerable<IMigrationEntity> GetDataLetters(MigrationArgs args)
        {
            return ShexieRepository.GetAttachments()
                .Where(x => !string.IsNullOrWhiteSpace(x.Path) && x.Path.Contains(LetterPath) && IsWordFile(x.Path))
                .Select(o => (IMigrationEntity)o.ToMigrationLetter(ShexieSettingsService.DocumentsPath));
        }

        private IEnumerable<IMigrationEntity> GetDataRecalls(MigrationArgs args)
        {
            return ShexieRepository.GetRecalls()
                .Select(o => (IMigrationEntity)o.ToMigrationRecall());
        }

        private IEnumerable<IMigrationEntity> GetDataAllergies(MigrationArgs args)
        {
            var allergyAnalyses = ShexieRepository.GetAnalyses()
                .Where(x => string.Equals(x.Desc, AllergiesName, StringComparison.InvariantCultureIgnoreCase));

            var allergies = ShexieRepository.GetStatistics()
                .Where(x => allergyAnalyses.Any(y => y.Id == x.Type))
                .GroupBy(o => o.PatientId, o => o.Analysis.Desc)
                .ToArray();

            return allergies.Select(o => (IMigrationEntity)ShexieMigrationHelper.ToMigrationAllergy(o));
        }

        private IEnumerable<IMigrationEntity> GetDataInterestedParties(MigrationArgs args)
        {
            return ShexieRepository.GetInterestedParties()
                .Select(o => (IMigrationEntity)o.ToMigrationInterestedParty());
        }

        private IEnumerable<IMigrationEntity> GetDataConsults(MigrationArgs args)
        {
            return ShexieRepository.GetPatientHistories()
                .Where(x => x.Formatted == (int)ShexieMigrationHelper.ShexieHistoryNoteFormatted.RtfTextForConsultHistory)
                .Select(o => (IMigrationEntity)o.ToMigrationConsult());
        }

        private static bool IsWordFile(string filepath)
        {
            var wordExtensions = new[] { ".docx" };
            return wordExtensions.Any(x => x.Equals(Path.GetExtension(filepath), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}