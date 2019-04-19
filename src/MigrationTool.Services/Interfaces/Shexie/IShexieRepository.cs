using System.Collections.Generic;

using MigrationTool.Services.Entities.Shexie;

namespace MigrationTool.Services.Interfaces.Shexie
{
    internal interface IShexieRepository
    {
        IEnumerable<ShexieUser> GetUsers();

        IEnumerable<ShexieProvider> GetProviders();

        IEnumerable<ShexieContact> GetContacts();

        IEnumerable<ShexieCompany> GetCompanies();

        IEnumerable<ShexiePatient> GetPatients();

        IEnumerable<ShexiePatientHistory> GetPatientHistories();

        IEnumerable<ShexieOpReport> GetOpReports();

        IEnumerable<ShexieLaboratoryResult> GetLaboratoryResults();

        IEnumerable<ShexieReferral> GetReferrals();

        IEnumerable<ShexieAlarm> GetAlarms();

        IEnumerable<ShexieAppointmentType> GetAppointmentTypes();

        IEnumerable<ShexieAppointment> GetAppointments();

        IEnumerable<ShexieAttachment> GetAttachments();

        IEnumerable<ShexieScript> GetScripts();

        IEnumerable<ShexieRecall> GetRecalls();

        IEnumerable<ShexieStatistic> GetStatistics();

        IEnumerable<ShexieAnalysis> GetAnalyses();

        IEnumerable<ShexieInterestedParty> GetInterestedParties();
    }
}