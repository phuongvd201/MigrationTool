namespace MigrationTool.ViewModels.Interfaces
{
    public enum ApplicationState
    {
        NotLoggedIn,
        LoginInProgress,
        SourceSystemSelection,
        GenieOptions,
        ZedmedOptions,
        ShexieOptions,
        C2cXmlOptions,
        C2cXmlValidation,
        MigrationOptions,
        MigrationInProgress,
        MigrationResult,
        MedicalDirectorOptions,
    }
}