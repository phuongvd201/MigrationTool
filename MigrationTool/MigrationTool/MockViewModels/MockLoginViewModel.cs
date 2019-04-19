using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockLoginViewModel : ILoginViewModel
    {
        public MockLoginViewModel()
        {
            var mockUsername = new Mock<IInputProperty<string>>();
            mockUsername.Setup(x => x.Input).Returns("testuser@domain.com");
            Username = mockUsername.Object;

            var mockState = new Mock<IProperty<ApplicationState>>();
            mockState.Setup(x => x.Value).Returns(ViewModels.Interfaces.ApplicationState.NotLoggedIn);

            var mockApplicationStateViewModel = new Mock<IApplicationStateViewModel>();
            mockApplicationStateViewModel.Setup(x => x.State).Returns(mockState.Object);
            ApplicationState = mockApplicationStateViewModel.Object;

            var mockLoginResultMessage = new Mock<IProperty<string>>();
            mockLoginResultMessage.Setup(x => x.Value).Returns("Invalid user name or password.");
            LoginResultMessage = mockLoginResultMessage.Object;
        }

        public IInputProperty<string> Username { get; private set; }

        public IApplicationStateViewModel ApplicationState { get; private set; }

        public IProperty<string> LoginResultMessage { get; private set; }

        public IProperty<ICommand> LoginCommand
        {
            get { return null; }
        }
    }
}