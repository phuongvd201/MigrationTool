using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IMainWindowViewModel
    {
        IApplicationStateViewModel ApplicationState { get; }

        IProperty<string> Version { get; }

        IProperty<ILoginViewModel> Login { get; }

        IProperty<ISourceSystemSelectionViewModel> SourceSystemSelection { get; }

        IProperty<IGenieOptionsViewModel> GenieOptions { get; }

        IProperty<IZedmedOptionsViewModel> ZedmedOptions { get; }

        IProperty<IShexieOptionsViewModel> ShexieOptions { get; }

        IProperty<IMigrationOptionsViewModel> MigrationOptions { get; }

        IProperty<IMigrationProgressViewModel> MigrationProgress { get; }

        IProperty<IMedicalDirectorOptionsViewModel> MedicalDirectorOptions { get; }
    }
}