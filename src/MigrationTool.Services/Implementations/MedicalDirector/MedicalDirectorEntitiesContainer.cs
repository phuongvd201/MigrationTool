using System;
using System.Collections.Generic;
using System.Linq;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers.MedicalDirector;
using MigrationTool.Services.Interfaces.MedicalDirector;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations.MedicalDirector
{
    internal class MedicalDirectorEntitiesContainer : IMedicalDirectorEntitiesContainer
    {
        public IMedicalDirectorRepository MedicalDirectorRepository { get; set; }

        public MigrationEntity[] SupportedEntityTypes
        {
            get { return DataSources.Keys.ToArray(); }
        }

        public MigrationSourceSystem SourceSystem
        {
            get { return MigrationSourceSystem.MedicalDirector; }
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
                        ////{ MigrationEntity.Allergies, GetDataAllergies },
                        ////{ MigrationEntity.Appointments, GetDataAppointments },
                        ////{ MigrationEntity.AppointmentTypes, GetDataAppointmentTypes },
                        ////{ MigrationEntity.ChecklistTemplates, GetDataChecklistTemplates },
                        ////{ MigrationEntity.Checklists, GetDataChecklists },
                        ////{ MigrationEntity.Consults, GetDataConsults },
                        { MigrationEntity.Contacts, GetDataContacts },
                        ////{ MigrationEntity.Documents, GetDataDocuments },
                        ////{ MigrationEntity.LaboratoryResults, GetDataLaboratoryResults },
                        ////{ MigrationEntity.Letters, GetDataLetters },
                        ////{ MigrationEntity.Measurements, GetDataMeasurements },
                        { MigrationEntity.Patients, GetDataPatients },
                        { MigrationEntity.Referrals, GetDataReferrals },
                        ////{ MigrationEntity.Tasks, GetDataTasks },
                        { MigrationEntity.Users, GetDataUsers },
                        ////{ MigrationEntity.Scripts, GetDataScripts },
                        ////{ MigrationEntity.Vaccinations, GetDataVaccinations },
                        ////{ MigrationEntity.InterestedParties, GetDataInterestedParties },
                        ////{ MigrationEntity.Companies, GetDataCompany },
                    };
                }
                return localDataSources;
            }
        }

        private IEnumerable<IMigrationEntity> GetDataReferrals(MigrationArgs arg)
        {
            var contacts = MedicalDirectorRepository.GetAddressBookEntries()
                .GroupBy(x => MedicalDirectorMigrationHelper.ToFullName(x.Title, x.FirstName, x.Surname))
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            return MedicalDirectorRepository.GetReferrals()
                .Select(x => (IMigrationEntity)x.ToMigrationReferral(contacts));
        }

        private IEnumerable<IMigrationEntity> GetDataAccountHolders(MigrationArgs arg)
        {
            var patients = MedicalDirectorRepository.GetPatients().ToArray();

            var payerIds = patients
                .GroupBy(x => x.PayerId)
                .Where(x => x.Key.HasValue)
                .ToDictionary(x => x.Key.Value, x => x.Select(y => y.Id).ToArray());

            var accountHolders = patients.Where(x => payerIds.ContainsKey(x.Id));

            return accountHolders
                .Select(x => (IMigrationEntity)x.ToMigrationAccountHolder(payerIds));
        }

        private IEnumerable<IMigrationEntity> GetDataUsers(MigrationArgs arg)
        {
            return MedicalDirectorRepository.GetResources()
                .Select(x => (IMigrationEntity)x.ToMigrationUser());
        }

        private IEnumerable<IMigrationEntity> GetDataContacts(MigrationArgs arg)
        {
            return MedicalDirectorRepository.GetAddressBookEntries()
                .Select(x => (IMigrationEntity)x.ToMigrationContact());
        }

        private IEnumerable<IMigrationEntity> GetDataPatients(MigrationArgs arg)
        {
            var medicalDirectorAhiaTrades = MedicalDirectorRepository.GetAhiaTrades().ToDictionary(x => x.Id, x => x.Trading);
            var medicalDirectorCountries = MedicalDirectorRepository.GetCountries().ToDictionary(x => x.Id, x => x.Name);
            var medicalDirectorLanguages = MedicalDirectorRepository.GetLanguages().ToDictionary(x => x.Id, x => x.Language);

            return MedicalDirectorRepository.GetPatients()
                .Select(x => (IMigrationEntity)x.ToMigrationPatient(medicalDirectorAhiaTrades, medicalDirectorCountries, medicalDirectorLanguages));
        }
    }
}