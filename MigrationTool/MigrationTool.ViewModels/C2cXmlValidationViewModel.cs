using System.Windows.Input;

using MigrationTool.Services.Interfaces.C2cXml;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class C2cXmlValidationViewModel : IC2cXmlValidationViewModel
    {
        public C2cXmlValidationViewModel()
        {
            localValidationResult = Reloadable<string>
                .On().Worker().Each().Call(x => C2cXmlValidationService.Validate())
                .Create();

            StartValidationCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(o =>
                    {
                        localValidationResult.Go();
                    })).Create();

            BackCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(o =>
                        ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();

            localValidationResult.OnChanged(() =>
            {
                if (string.IsNullOrWhiteSpace(localValidationResult.Value))
                {
                    ApplicationState.DoStateAction(ApplicationStateAction.Next);
                }
            });
        }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public IC2cXmlValidationService C2cXmlValidationService { get; set; }

        private readonly ICallProperty<string> localValidationResult;

        public IProperty<string> ValidationResult
        {
            get { return localValidationResult; }
        }

        public IProperty<ICommand> StartValidationCommand { get; private set; }

        public IProperty<ICommand> BackCommand { get; private set; }
    }
}