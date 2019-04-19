using System;
using System.Windows.Input;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class LoginViewModel : ILoginViewModel
    {
        public LoginViewModel(bool testMode = false)
        {
            Username = Reloadable<string>.On().Each().Input().Create();

            LoginCommand = Reloadable<ICommand>
                .On().First().Get(x =>
                    new DelegateCommand(o =>
                    {
                        localCredentials = new Credentials
                        {
                            Username = Username.Value,
                            Password = (Func<string>)o,
                        };
                        ApplicationState.DoStateAction(ApplicationStateAction.Next);
                        localLoginResultMessage.Go();
                    }))
                .Create();

            localLoginResultMessage = Reloadable<string>
                .On().Worker(testMode).Each().Call(x =>
                    AuthenticationService.Login(localCredentials))
                .Create(string.Empty);

            localLoginResultMessage.OnChanged(() => ApplicationState.DoStateAction(string.IsNullOrWhiteSpace(localLoginResultMessage.Value) ? ApplicationStateAction.Next : ApplicationStateAction.Back));
        }

        public IAuthenticationService AuthenticationService { get; set; }

        public IApplicationStateViewModel ApplicationState { get; set; }

        private readonly ICallProperty<string> localLoginResultMessage;

        private Credentials localCredentials;

        public IProperty<string> LoginResultMessage
        {
            get { return localLoginResultMessage; }
        }

        public IInputProperty<string> Username { get; private set; }

        public IProperty<ICommand> LoginCommand { get; private set; }
    }
}