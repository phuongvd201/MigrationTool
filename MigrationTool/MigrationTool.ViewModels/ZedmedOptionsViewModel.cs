using System;
using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Interfaces;
using MigrationTool.Services.Interfaces.Zedmed;
using MigrationTool.ViewModels.Interfaces;

using Property;
using Property.Windows;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class ZedmedOptionsViewModel : IZedmedOptionsViewModel
    {
        public ZedmedOptionsViewModel()
        {
            var stringInputPrototype = Reloadable<string>
                .On().Each().Input();

            DatabaseFolderPath = Reloadable<string>
                .On().Each().ValidationInput(x =>
                    ShowValidationResults.Value
                        ? ValidationService.ValidateSettings(MigrationSourceSystem.Zedmed)
                        : Enumerable.Empty<string>())
                .Create();
            DocumentsPath = stringInputPrototype.Create();
            Username = stringInputPrototype.Create();

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

            DatabaseFolderPath.OnChanged(() =>
            {
                ZedmedSettingsService.DatabaseFolderPath = DatabaseFolderPath.Value;
            });

            ShowValidationResults = Reloadable<bool>.On().Each().Call(x => !x).Create(false);

            ShowValidationResults.OnChanged(() =>
            {
                if (!ShowValidationResults.Value)
                {
                    return;
                }
                DatabaseFolderPath.Validate();
                ShowValidationResults.Go();
            });

            ReadConfigurationCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(
                        o =>
                        {
                            Username.Input = ZedmedSettingsService.Username;
                            DatabaseFolderPath.Input = ZedmedSettingsService.DatabaseFolderPath;
                            DocumentsPath.Input = ZedmedSettingsService.DocumentsPath;
                        },
                        y => !localErrors.IsLoading))
                .Create();

            NextCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(
                        o =>
                        {
                            ZedmedSettingsService.DatabaseFolderPath = DatabaseFolderPath.Value;
                            ZedmedSettingsService.DocumentsPath = DocumentsPath.Value;
                            ShowValidationResults.Go();
                            if (DatabaseFolderPath.HasErrors)
                            {
                                return;
                            }
                            ZedmedSettingsService.Username = Username.Value;
                            ZedmedSettingsService.Password = () => ((Func<string>)o)();
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

        public IValidationService ValidationService { get; set; }

        private readonly ICallProperty<string[]> localErrors;

        public IProperty<string[]> Errors
        {
            get { return localErrors; }
        }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IZedmedSettingsService ZedmedSettingsService { get; set; }

        public IZedmedConnectionTestService ConnectionService { get; set; }

        public IValidationInputProperty<string> DatabaseFolderPath { get; private set; }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IInputProperty<string> Username { get; private set; }

        public IProperty<ICommand> ReadConfigurationCommand { get; private set; }

        public IProperty<ICommand> BackCommand { get; private set; }

        public IProperty<ICommand> NextCommand { get; private set; }
    }
}