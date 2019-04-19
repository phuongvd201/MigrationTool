using System;

using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class MainWindowViewModel : IMainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Version = Reloadable<string>.On().First().Get(x => AppSettingsService.Version).Create();
            Login = Reloadable<ILoginViewModel>.On().First().Get(x => GetLoginViewModel()).Create();
            SourceSystemSelection = Reloadable<ISourceSystemSelectionViewModel>.On().First().Get(x => GetSourceSystemSelectionViewModel()).Create();
            GenieOptions = Reloadable<IGenieOptionsViewModel>.On().First().Get(x => GetGenieOptionsViewModel()).Create();
            ZedmedOptions = Reloadable<IZedmedOptionsViewModel>.On().First().Get(x => GetZedmedOptionsViewModel()).Create();
            ShexieOptions = Reloadable<IShexieOptionsViewModel>.On().First().Get(x => GetShexieOptionsViewModel()).Create();
            C2cXmlOptions = Reloadable<IC2cXmlOptionsViewModel>.On().First().Get(x => GetC2cXmlOptionsViewModel()).Create();
            C2cXmlValidation = Reloadable<IC2cXmlValidationViewModel>.On().First().Get(x => GetC2cXmlValidationViewModel()).Create();
            MigrationOptions = Reloadable<IMigrationOptionsViewModel>.On().First().Get(x => GetMigrationOptionsViewModel()).Create();
            MigrationProgress = Reloadable<IMigrationProgressViewModel>.On().First().Get(x => GetMigrationProgressViewModel()).Create();
            MedicalDirectorOptions = Reloadable<IMedicalDirectorOptionsViewModel>.On().First().Get(x => GetMedicalDirectorOptionsViewModel()).Create();
        }

        public IAppSettingsService AppSettingsService { get; set; }

        public Func<ILoginViewModel> GetLoginViewModel { get; set; }

        public Func<ISourceSystemSelectionViewModel> GetSourceSystemSelectionViewModel { get; set; }

        public Func<IGenieOptionsViewModel> GetGenieOptionsViewModel { get; set; }

        public Func<IZedmedOptionsViewModel> GetZedmedOptionsViewModel { get; set; }

        public Func<IShexieOptionsViewModel> GetShexieOptionsViewModel { get; set; }

        public Func<IC2cXmlOptionsViewModel> GetC2cXmlOptionsViewModel { get; set; }

        public Func<IC2cXmlValidationViewModel> GetC2cXmlValidationViewModel { get; set; }

        public Func<IMigrationOptionsViewModel> GetMigrationOptionsViewModel { get; set; }

        public Func<IMigrationProgressViewModel> GetMigrationProgressViewModel { get; set; }

        public Func<IMedicalDirectorOptionsViewModel> GetMedicalDirectorOptionsViewModel { get; set; }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IProperty<string> Version { get; private set; }

        public IProperty<ILoginViewModel> Login { get; private set; }

        public IProperty<ISourceSystemSelectionViewModel> SourceSystemSelection { get; private set; }

        public IProperty<IGenieOptionsViewModel> GenieOptions { get; private set; }

        public IProperty<IZedmedOptionsViewModel> ZedmedOptions { get; private set; }

        public IProperty<IShexieOptionsViewModel> ShexieOptions { get; private set; }

        public IProperty<IC2cXmlOptionsViewModel> C2cXmlOptions { get; private set; }

        public IProperty<IC2cXmlValidationViewModel> C2cXmlValidation { get; private set; }

        public IProperty<IMigrationOptionsViewModel> MigrationOptions { get; private set; }

        public IProperty<IMigrationProgressViewModel> MigrationProgress { get; private set; }

        public IProperty<IMedicalDirectorOptionsViewModel> MedicalDirectorOptions { get; private set; }
    }
}