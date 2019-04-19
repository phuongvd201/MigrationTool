using System;
using System.Collections.Generic;

using MigrationTool.Services.Entities.Zedmed;

namespace MigrationTool.Services.Interfaces.Zedmed
{
    internal interface IZedmedRepository
    {
        IEnumerable<ZedmedUser> GetUsers();

        IEnumerable<ZedmedPatient> GetPatients();

        IEnumerable<ZedmedPatientProblem> GetPatientProblems();

        IEnumerable<ZedmedReferral> GetReferrals();

        IEnumerable<ZedmedAppointment> GetAppointments(DateTime? start);

        IEnumerable<ZedmedAppointmentType> GetAppointmentTypes();

        IEnumerable<ZedmedContact> GetContacts();

        IEnumerable<ZedmedEncounter> GetEncounters(DateTime? start);

        IEnumerable<ZedmedEncounterNote> GetEncounterNotes();

        IEnumerable<ZedmedAllergy> GetAllergies();

        IEnumerable<ZedmedTemplateControl> GetTemplateControls();

        IEnumerable<ZedmedMeasurement> GetMeasurements(DateTime? start);

        IEnumerable<ZedmedAccountHolder> GetAccountHolders();

        IEnumerable<ZedmedChecklistTemplate> GetChecklistTemplates();

        IEnumerable<ZedmedInterestedParty> GetInterestedParties();

        IEnumerable<ZedmedVaccination> GetVaccinations(DateTime? start);

        IEnumerable<ZedmedAccountPayer> GetAccountPayers();

        IEnumerable<ZedmedAttachment> GetAttachments(DateTime? start);

        IEnumerable<ZedmedImageDocument> GetImageDocuments();

        IEnumerable<ZedmedLetter> GetLetters();

        IEnumerable<ZedmedDrug> GetDrugs();

        IEnumerable<ZedmedScript> GetScripts();

        IEnumerable<ZedmedLaboratoryResult> GetLaboratoryResults();

        IEnumerable<ZedmedTask> GetTasks(DateTime? start);

        IEnumerable<ZedmedRecall> GetRecalls();

        IEnumerable<ZedmedWorkCoverClaim> GetWorkCoverClaims();
    }
}