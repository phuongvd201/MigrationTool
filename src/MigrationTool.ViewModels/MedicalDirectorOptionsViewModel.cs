using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Interfaces.MedicalDirector;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class MedicalDirectorOptionsViewModel : IMedicalDirectorOptionsViewModel
    {
        public MedicalDirectorOptionsViewModel()
        {
            DocumentsPath = Reloadable<string>.On().Each().Input().Create();
            DatabaseServerName = Reloadable<string>.On().Each().Input().Create();

            localErrors = Reloadable<string[]>
                .On().Worker().Each().Call(x =>
                    ConnectionService.TestConnection()
                        ? new string[]
                        {
                        }
                        : new[] { "Invalid connection settings." })
                .Create();

            localErrors.OnChanged(() =>
            {
                if (localErrors.Value.Any())
                {
                    return;
                }
                ApplicationState.DoStateAction(ApplicationStateAction.Next);
            });

            ShowValidationResults = Reloadable<bool>.On().Each().Call(x => !x).Create(false);

            ReadConfigurationCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(
                        o =>
                        {
                            DatabaseServerName.Input = MedicalDirectorSettingsService.DatabaseServerName;
                            DocumentsPath.Input = MedicalDirectorSettingsService.DocumentsPath;
                        },
                        y => !localErrors.IsLoading))
                .Create();

            NextCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(
                        o =>
                        {
                            MedicalDirectorSettingsService.DocumentsPath = DocumentsPath.Value;
                            MedicalDirectorSettingsService.DatabaseServerName = DatabaseServerName.Value;
                            ShowValidationResults.Go();
                            localErrors.Go();
                        },
                        y => !localErrors.IsLoading))
                .Create();

            BackCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(o => ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();
        }

        private ICallProperty<bool> ShowValidationResults { get; set; }

        private readonly ICallProperty<string[]> localErrors;

        public IProperty<string[]> Errors
        {
            get { return localErrors; }
        }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IMedicalDirectorSettingsService MedicalDirectorSettingsService { get; set; }

        public IMedicalDirectorConnectionTestService ConnectionService { get; set; }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IInputProperty<string> DatabaseServerName { get; private set; }

        public IProperty<ICommand> ReadConfigurationCommand { get; private set; }

        public IProperty<ICommand> BackCommand { get; private set; }

        public IProperty<ICommand> NextCommand { get; private set; }
    }
}