using System.Collections.Generic;

using MigrationTool.Services.Entities.MedicalDirector;

namespace MigrationTool.Services.Interfaces.MedicalDirector
{
    internal interface IMedicalDirectorRepository
    {
        IEnumerable<MedicalDirectorResource> GetResources();

        IEnumerable<MedicalDirectorReferral> GetReferrals();

        IEnumerable<MedicalDirectorAddressBookEntry> GetAddressBookEntries();

        IEnumerable<MedicalDirectorPatient> GetPatients();

        IEnumerable<MedicalDirectorAhiaTrade> GetAhiaTrades();

        IEnumerable<MedicalDirectorCountry> GetCountries();

        IEnumerable<MedicalDirectorLanguage> GetLanguages();
    }
}