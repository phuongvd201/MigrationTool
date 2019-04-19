using System;
using System.Collections.Generic;

using MigrationTool.Services.Entities.Genie;

namespace MigrationTool.Services.Interfaces.Genie
{
    internal interface IGenieRepository
    {
        IEnumerable<GenieUser> GetUsers();

        IEnumerable<GenieRecall> GetRecalls();

        IEnumerable<GenieCurrentProblem> GetCurrentProblems();

        IEnumerable<GenieConsultProblem> GetConsultProblems();

        IEnumerable<GeniePastHistory> GetPastHistory();

        IEnumerable<GeniePatient> GetPatients(DateTime? start);

        IEnumerable<GenieReferral> GetReferrals();

        IEnumerable<GenieAppointment> GetAppointments(DateTime? start);

        IEnumerable<GenieAppointmentType> GetAppointmentTypes();

        IEnumerable<GenieContact> GetContacts();

        IEnumerable<GenieConsult> GetConsults(DateTime? start);

        IEnumerable<GenieAllergy> GetAllergies();

        IEnumerable<GenieMeasurement> GetMeasurements(DateTime? start);

        IEnumerable<GenieOutgoingLetter> GetOutgoingLetters(DateTime? start);

        IEnumerable<GenieIncomingLetter> GetIncomingLetters(DateTime? start);

        IEnumerable<GenieDownloadedResult> GetDownloadedResults(DateTime? start);

        IEnumerable<GenieGraphic> GetGraphics(DateTime? start);

        IEnumerable<GenieAccountHolder> GetAccountHolders();

        IEnumerable<GenieTask> GetTasks(DateTime? start);

        IEnumerable<GenieChecklist> GetPatientChecklists();

        IEnumerable<GenieChecklist> GetChecklistTemplates();

        IEnumerable<GenieChecklistField> GetChecklistFields();

        IEnumerable<GenieVaccination> GetVaccinations(DateTime? start);

        IEnumerable<GenieScript> GetScripts(DateTime? start);

        IEnumerable<GenieDrug> GetDrugs();

        IEnumerable<GenieQuoteItem> GetQuoteItems();

        IEnumerable<GenieOpReport> GetOpReports(DateTime? start);

        IEnumerable<GenieComplication> GetComplications(DateTime? start);

        IEnumerable<GenieAntenatalVisit> GetAntenatalVisits();

        IEnumerable<GeniePregnancy> GetOnGRecords();

        IEnumerable<GenieObstetricHistory> GetOnGHistoryRecords();

        IEnumerable<GenieInterestedParty> GetInterestedParties();

        IEnumerable<GenieUser> GetMd3Users();

        IEnumerable<GenieScript> GetMd3Scripts();

        IEnumerable<GenieDrug> GetMd3Drugs();

        IEnumerable<GenieEmployer> GetEmployers();

        IEnumerable<GenieAccountHolder> GetInsurers();

        IEnumerable<GenieWorkCoverClaim> GetWorkCoverClaims();

        IEnumerable<GenieDocument> GetDocuments();
    }
}