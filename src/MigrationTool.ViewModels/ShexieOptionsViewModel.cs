using System.Windows.Input;

using MigrationTool.Services.Interfaces.Shexie;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class ShexieOptionsViewModel : IShexieOptionsViewModel
    {
        public ShexieOptionsViewModel()
        {
            ReadConfigurationCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(o =>
                    {
                        DatabaseFilePath.Input = ShexieSettingsService.DatabaseFilePath;
                        DocumentsPath.Input = ShexieSettingsService.DocumentsPath;
                    })).Create();

            NextCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                {
                    ShexieSettingsService.DatabaseFilePath = DatabaseFilePath.Value;
                    ShexieSettingsService.DocumentsPath = DocumentsPath.Value;
                    ApplicationState.DoStateAction(ApplicationStateAction.Next);
                }))
                .Create();

            BackCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                    ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();

            DatabaseFilePath = Reloadable<string>.On().Each().Input().Create();
            DocumentsPath = Reloadable<string>.On().Each().Input().Create();
        }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IShexieSettingsService ShexieSettingsService { get; set; }

        public IInputProperty<string> DatabaseFilePath { get; private set; }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IProperty<ICommand> ReadConfigurationCommand { get; private set; }

        public IProperty<ICommand> BackCommand { get; private set; }

        public IProperty<ICommand> NextCommand { get; private set; }
    }
}