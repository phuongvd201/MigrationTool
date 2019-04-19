using System;
using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Interfaces.Genie;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class GenieOptionsViewModel : IGenieOptionsViewModel
    {
        public GenieOptionsViewModel()
        {
            var stringInputPrototype = Reloadable<string>
                .On().Each().Input();

            DocumentsPath = stringInputPrototype.Create();
            LaboratoryResultsPath = stringInputPrototype.Create();
            XmlExportPath = stringInputPrototype.Create();
            Md3XmlPath = stringInputPrototype.Create();
            Ip = stringInputPrototype.Create();
            Port = stringInputPrototype.Create();
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

            ReadConfigurationCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(
                        o =>
                        {
                            Ip.Input = GenieSettingsService.IP;
                            Port.Input = GenieSettingsService.Port;
                            Username.Input = GenieSettingsService.Username;
                            XmlExportPath.Input = GenieSettingsService.XmlExportPath;
                            Md3XmlPath.Input = GenieSettingsService.Md3XmlPath;
                            DocumentsPath.Input = GenieSettingsService.DocumentsPath;
                            LaboratoryResultsPath.Input = GenieSettingsService.LaboratoryResultsPath;
                        },
                        y => !localErrors.IsLoading))
                .Create();

            NextCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(
                        o =>
                        {
                            GenieSettingsService.IP = Ip.Value;
                            GenieSettingsService.Port = Port.Value;
                            GenieSettingsService.Username = Username.Value;
                            GenieSettingsService.Password = () => ((Func<string>)o)();
                            GenieSettingsService.XmlExportPath = XmlExportPath.Value;
                            GenieSettingsService.Md3XmlPath = Md3XmlPath.Value;
                            GenieSettingsService.DocumentsPath = DocumentsPath.Value;
                            GenieSettingsService.LaboratoryResultsPath = LaboratoryResultsPath.Value;
                            localErrors.Go();
                        },
                        y => !localErrors.IsLoading))
                .Create();

            BackCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(o => ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();
        }

        private readonly ICallProperty<string[]> localErrors;

        public IProperty<string[]> Errors
        {
            get { return localErrors; }
        }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IGenieSettingsService GenieSettingsService { get; set; }

        public IGenieConnectionTestService ConnectionService { get; set; }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IInputProperty<string> LaboratoryResultsPath { get; private set; }

        public IInputProperty<string> XmlExportPath { get; private set; }

        public IInputProperty<string> Md3XmlPath { get; private set; }

        public IInputProperty<string> Ip { get; private set; }

        public IInputProperty<string> Port { get; private set; }

        public IInputProperty<string> Username { get; private set; }

        public IProperty<ICommand> ReadConfigurationCommand { get; private set; }

        public IProperty<ICommand> BackCommand { get; private set; }

        public IProperty<ICommand> NextCommand { get; private set; }
    }
}