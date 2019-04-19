using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Interfaces;
using MigrationTool.Services.Interfaces.C2cXml;
using MigrationTool.ViewModels.Interfaces;

using Property;
using Property.Windows;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class C2cXmlOptionsViewModel : IC2cXmlOptionsViewModel
    {
        public C2cXmlOptionsViewModel()
        {
            ShowValidationResults = Reloadable<bool>.On().Each().Call(x => !x).Create(false);

            ShowValidationResults.OnChanged(() =>
            {
                if (!ShowValidationResults.Value)
                {
                    return;
                }
                C2cXmlPath.Validate();
                ShowValidationResults.Go();
            });

            ReadConfigurationCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                {
                    C2cXmlPath.Input = C2cXmlSettingsService.C2cXmlPath;
                    C2cDocumentsPath.Input = C2cXmlSettingsService.C2cDocumentsPath;
                })).Create();

            NextCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                {
                    C2cXmlSettingsService.C2cDocumentsPath = C2cDocumentsPath.Value;
                    C2cXmlSettingsService.C2cXmlPath = C2cXmlPath.Value;
                    ShowValidationResults.Go();
                    if (C2cXmlPath.HasErrors)
                    {
                        return;
                    }
                    ApplicationState.DoStateAction(ApplicationStateAction.Next);
                })).Create();

            BackCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                    ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();

            C2cDocumentsPath = Reloadable<string>.On().Each().Input().Create();

            C2cXmlPath = Reloadable<string>.On().Each().ValidationInput(x =>
                ShowValidationResults.Value
                    ? ValidationService.ValidateSettings(MigrationSourceSystem.C2cXml)
                    : Enumerable.Empty<string>()).Create();

            C2cXmlPath.OnChanged(() =>
            {
                C2cXmlSettingsService.C2cXmlPath = C2cXmlPath.Value;
            });
        }

        private ICallProperty<bool> ShowValidationResults { get; set; }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IC2cXmlSettingsService C2cXmlSettingsService { get; set; }

        public IValidationService ValidationService { get; set; }

        public IValidationInputProperty<string> C2cXmlPath { get; private set; }

        public IInputProperty<string> C2cDocumentsPath { get; private set; }

        public IProperty<ICommand> ReadConfigurationCommand { get; private set; }

        public IProperty<ICommand> BackCommand { get; private set; }

        public IProperty<ICommand> NextCommand { get; private set; }
    }
}