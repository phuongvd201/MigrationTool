using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockC2cXmlValidationViewModel : IC2cXmlValidationViewModel
    {
        public MockC2cXmlValidationViewModel()
        {
            var mockValidationResult = new Mock<IProperty<string>>();
            mockValidationResult.Setup(x => x.Value)
                .Returns("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed"
                         + " do eiusmod tempor incididunt ut labore et dolore magna aliqua."
                         + " Ut enim ad minim veniam, quis nostrud exercitation ullamco"
                         + " laboris nisi ut aliquip ex ea commodo consequat. Duis aute"
                         + " irure dolor in reprehenderit in voluptate velit esse"
                         + " cillum dolore eu fugiat nulla pariatur. Excepteur"
                         + " sint occaecat cupidatat non proident, sunt"
                         + " in culpa qui officia deserunt mollit anim id est laborum.");
            mockValidationResult.Setup(x => x.IsLoading).Returns(false);

            ValidationResult = mockValidationResult.Object;
        }

        public IProperty<string> ValidationResult { get; private set; }

        public IProperty<ICommand> StartValidationCommand
        {
            get { return null; }
        }

        public IProperty<ICommand> BackCommand
        {
            get { return null; }
        }
    }
}